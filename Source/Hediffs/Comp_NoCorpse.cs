using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_NoCorpse : HediffCompProperties
    {
        // 死亡时留下的污秽
        public ThingDef filthDef = null;
        // 死亡时留下的物品
        public ThingDef itemDef = null;
        // 造成爆炸的类型
        public DamageDef damageDef = null;
        // 护甲穿透 
        public float armorPenetrationBase = 0;
        // 爆炸伤害数值
        public float damageAmountBase = 0;

        // 影响的范围(污秽和爆炸)
        public float radius = 1;
        // 爆炸的音效
        public SoundDef soundDef = null;

        public bool deathOnRemove = false;

        public HediffCompProperties_NoCorpse()
        {
            compClass = typeof(HediffComp_NoCorpse);
        }
    }

    public class HediffComp_NoCorpse : HediffComp
    {
        private HediffCompProperties_NoCorpse Props => (HediffCompProperties_NoCorpse)props;
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {

            base.Notify_PawnDied(dinfo, culprit);
            IntVec3 position = this.parent.pawn.Corpse.Position;
            Map cMap = this.parent.pawn.Corpse.Map;
            if (position == null) return;
            // 爆炸
            if (Props.damageDef != null)
            {
                DamageInfo dinfo2 = new DamageInfo(Props.damageDef, Props.damageAmountBase, Props.armorPenetrationBase, -1, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, this.parent.pawn);
                GenExplosion.DoExplosion(position, this.parent.pawn.Map, Props.radius, Props.damageDef, dinfo2.Instigator, -1, -1f, null, null, null, null, null, doSoundEffects: false);
            }

            // 污秽
            if (Props.filthDef != null)
            {
                // 获取范围内的所有Cell
                List<IntVec3> cells = GenRadial.RadialCellsAround(position, Props.radius, true).ToList();
                // 遍历每个Cell，生成污秽
                foreach (IntVec3 cell in cells)
                {
                    // 检查Cell是否在地图上
                    if (cell.InBounds(cMap))
                    {
                        // 生成污秽
                        FilthMaker.TryMakeFilth(cell, cMap, Props.filthDef, 1);
                    }
                }
            }
            // 留下物品
            if (Props.itemDef != null)
            {
                Thing item = ThingMaker.MakeThing(Props.itemDef, null);
                GenPlace.TryPlaceThing(item, position, this.parent.pawn.Map, ThingPlaceMode.Near);
            }

            // 音效
            if (Props.soundDef != null)
            {
                Props.soundDef.PlayOneShot(new TargetInfo(position, this.parent.pawn.Map, false));
            }

            // 删除尸体
            if (this.parent.pawn?.Corpse != null)
            {
                this.parent.pawn.Corpse.Destroy(DestroyMode.Vanish);
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (Props.deathOnRemove)
            {
                this.parent.pawn?.Kill(null, null);
            }
        }
    }
}
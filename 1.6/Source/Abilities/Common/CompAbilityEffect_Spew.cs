using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace NzFaceLessManMod
{
    // CompAbilityEffect_FireSpew
    public class CompAbilityEffect_Spew : Abstract_CompAbilityEffect_Spew
    {
        private new CompProperties_AbilitySpew Props => (CompProperties_AbilitySpew)props;

        private Pawn Caster => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {


            List<IntVec3> affected = base.AffectedCells(target);
            GenExplosion.DoExplosion(target.Cell, parent.pawn.MapHeld, 0f, Props.damageType, Caster,
             postExplosionSpawnThingDef: Props.filthDef, damAmount: Prop_DamAmount, armorPenetration: Props.armorPenetration, explosionSound: null, weapon: null,
              projectile: null, intendedTarget: null, postExplosionSpawnChance: 1f, postExplosionSpawnThingCount: 1, postExplosionGasType: null,
               applyDamageToExplosionCellsNeighbors: false, preExplosionSpawnThingDef: null, preExplosionSpawnChance: 0f, preExplosionSpawnThingCount: 1,
                chanceToStartFire: 0f, damageFalloff: false, direction: null, ignoredThings: null, affectedAngle: null, doVisualEffects: false,
                 propagationSpeed: 0.6f, excludeRadius: 0f, doSoundEffects: false, postExplosionSpawnThingDefWater: null, screenShakeFactor: 1f,
                  flammabilityChanceCurve: parent.verb.verbProps.flammabilityAttachFireChanceCurve, overrideCells: affected);

            // 如果有Hediff
            if (Props.hediff != null)
            { 
                var pawnNum = Utils.ApplyPawnInAffectedArea(Caster.Map, affected, delegate (Pawn p)
                {
                    if (p != Caster)
                    {
                        // 附加一个30秒的剧痛Hediff
                        Hediff hediff = HediffMaker.MakeHediff(Props.hediff, p);
                        p.health.AddHediff(hediff);
                    }
                });

                #if DEBUG
                Log.Message("Spew_Apply PawnNum: " + pawnNum);
                #endif
            }


            base.Apply(target, dest);
        }
    }

}

/*
GenExplosion.DoExplosion

target.Cell: 爆炸发生的目标位置。
parent.pawn.MapHeld: 爆炸发生的地图。
0f: 爆炸半径（这里为0，表示没有实际爆炸范围）。
DamageDefOf.Flame: 爆炸造成的伤害类型（这里是火焰伤害）。
Pawn: 造成爆炸的对象。
postExplosionSpawnThingDef: Props.filthDef: 爆炸后生成的物品类型。
damAmount: Props.damAmount: 爆炸造成的伤害量。
armorPenetration: -1f: 爆炸的护甲穿透力（-1表示默认值）。
explosionSound: null: 爆炸声音（null表示默认声音）。
weapon: null: 造成爆炸的武器（null表示没有特定武器）。
projectile: null: 造成爆炸的弹药（null表示没有特定弹药）。
intendedTarget: null: 爆炸的目标（null表示没有特定目标）。
postExplosionSpawnChance: 1f: 爆炸后生成物品的概率（1表示100%）。
postExplosionSpawnThingCount: 1: 爆炸后生成物品的数量。
postExplosionGasType: null: 爆炸后生成的气体类型（null表示没有气体）。
applyDamageToExplosionCellsNeighbors: false: 是否对爆炸范围内的邻近单元格造成伤害。
preExplosionSpawnThingDef: null: 爆炸前生成的物品类型（null表示没有物品）。
preExplosionSpawnChance: 0f: 爆炸前生成物品的概率（0表示不会生成）。
preExplosionSpawnThingCount: 1: 爆炸前生成物品的数量。
chanceToStartFire: 1f: 爆炸后点燃火焰的概率（1表示100%）。
damageFalloff: false: 是否随距离衰减伤害。
direction: null: 爆炸方向（null表示无特定方向）。
ignoredThings: null: 被忽略的对象（null表示没有忽略对象）。
affectedAngle: null: 受影响的角度（null表示全方位）。
doVisualEffects: false: 是否显示视觉效果。
propagationSpeed: 0.6f: 爆炸传播速度。
excludeRadius: 0f: 排除的半径（0表示没有排除）。
doSoundEffects: false: 是否播放声音效果。
postExplosionSpawnThingDefWater: null: 爆炸后在水中生成的物品类型（null表示没有物品）。
screenShakeFactor: 1f: 屏幕震动因子。
flammabilityChanceCurve: parent.verb.verbProps.flammabilityAttachFireChanceCurve: 可燃性概率曲线。
overrideCells: AffectedCells(target): 覆盖的单元格。
*/
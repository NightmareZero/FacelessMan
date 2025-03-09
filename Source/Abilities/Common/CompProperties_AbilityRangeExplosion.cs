using Verse;
using RimWorld;
using System.Collections.Generic;

namespace NzFaceLessManMod
{


    public class CompProperties_AbilityRangeExplosion : CompProperties_AbilityEffect
    {

        // 目标误差范围
        public float deviationRange;

        // 单次爆炸范围
        public float explosionRange;

        // 遗留地板类型
        public ThingDef filthDef;

        // 伤害类型(多种) 不为空时优先使用
        public List<DamageDef> damageTypes = new List<DamageDef>();

        // 伤害类型
        public DamageDef damageType;

        // 伤害类型获取顺序 0. 随机 1. 顺序
        public int damageTypeOrder = 0;

        // 伤害次数
        public int damageTimes = 1;

        // 伤害值
        public int damage = 40;

        // 穿甲值
        public float armorPenetration = 1f;

        // 伤害放大系数
        public StatDef damageFactorStat;

        // 穿甲放大系数
        public StatDef armorPenetrationFactorStat;

        // 爆炸特效
        public EffecterDef effecterDef;

        // 爆炸后音效
        public SoundDef explosionSound;

        // 给受到影响的人添加hediff 不为空时优先使用
        public List<HediffDef> addHediffDefs = new List<HediffDef>();

        // 给受到影响的人添加hediff
        public HediffDef addHediffDef;


        public CompProperties_AbilityRangeExplosion()
        {
            compClass = typeof(CompAbilityEffect_RangeExplosion);

        }

        /// <summary>
        /// 获取伤害类型
        /// </summary>
        public List<DamageDef> GetDamageTypes()
        {
            if (damageTypes.Count == 0 && damageType != null)
            {
                damageTypes.Add(damageType);
            }

            if (damageTypes.Count == 0)
            {
                damageTypes.Add(DamageDefOf.Bomb);
            }
            return damageTypes;
        }

        /// <summary>
        /// 获取Hediff类型
        /// </summary>
        public List<HediffDef> GetHediffDefs()
        {
            if (addHediffDefs.Count == 0 && addHediffDef != null)
            {
                addHediffDefs.Add(addHediffDef);
            }
            return addHediffDefs;
        }

        /// <summary>
        /// 按照顺序获取伤害类型
        /// </summary>
        /// <returns></returns>
        public DamageDef GetOrderedDamageType()
        {
            DamageDef res = null;
            if (damageTypeOrder == 0)
            {
                _damageTypeIndex = Rand.Range(0, damageTypes.Count);
                res = GetDamageTypes()[_damageTypeIndex];
            }
            else
            {
                res = GetDamageTypes()[_damageTypeIndex];
                _damageTypeIndex = (_damageTypeIndex + 1) % damageTypes.Count;
            }

            return res;
        }
        private int _damageTypeIndex = 0;
    }
}
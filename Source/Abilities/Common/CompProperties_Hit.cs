using RimWorld;
using System;
using System.Linq;
using Verse;



namespace NzFaceLessManMod
{
    public class CompProperties_Hit : CompProperties_AbilityEffect
    { 
        // 伤害类型
        public DamageDef damageDef;
        public DamageDef DamageDef
        {
            get
            {
                if (damageDef == null)
                {
                    damageDef = DamageDefOf.Blunt;
                }
                return damageDef;
            }
        }

        // 伤害数值
        public float damageAmountBase;
        // 穿甲
        public float armorPenetrationBase = 0;
        // 伤害放大
        public StatDef damageMultiplierStat;
        // 穿甲放大
        public StatDef armorPenetrationMultiplierStat;
        // 打击部位
        public BodyPartDef hitPart = null;
        // 声音
        public SoundDef soundHitPawn;

        public SoundDef SoundHitPawn
        {
            get
            {
                if (soundHitPawn == null)
                {
                    soundHitPawn = SoundDefOf.Pawn_Melee_Punch_HitPawn;
                }
                return soundHitPawn;
            }
        }
        // 击晕时间
        public int stunTicks = 0;

        public CompProperties_Hit()
        {
            compClass = typeof(CompAbilityEffect_Hit);
        }


    }
}
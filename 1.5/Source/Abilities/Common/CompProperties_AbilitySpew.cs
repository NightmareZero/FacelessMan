using Verse;
using RimWorld;

namespace NzFaceLessManMod
{

    public class CompProperties_AbilitySpew : CompProperties_AbilityEffect
    {
        public float range;

        public StatDef rangeStat;

        public float lineWidthEnd;

        public ThingDef filthDef;

        public DamageDef damageType;

        public int damageAmount = -1;

        public float armorPenetration = 0.1f;

        public EffecterDef effecterDef;

        public StatDef damMultiplierStat;

        public bool canHitFilledCells;

        // 添加Hediff
        public HediffDef hediff;

        // public CompProperties_AbilitySpew()
        // {
        //     compClass = typeof(CompAbilityEffect_Spew);
        // }
    }
}
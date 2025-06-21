using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProperties_Healing : GeneExtCompProp
    {
        public IntRange healingIntervalTicksRange = new IntRange(300000, 600000);

        public CompProperties_Healing()
        {
            compClass = typeof(GeneExtComp_Healing);
        }
    }

    public class GeneExtComp_Healing : GeneExtComp
    {
        private int ticksToHeal;

        public CompProperties_Healing Props => (CompProperties_Healing)props;

        public override void CompPostPostAdd()
        {
            base.CompPostPostAdd();
            ResetInterval();
        }

        public override void CompPostTick()
        {
            base.CompPostTick();
            ticksToHeal--;
            if (ticksToHeal <= 0)
            {
                HediffComp_HealPermanentWounds.TryHealRandomPermanentWound(parent.pawn, parent.LabelCap);
                ResetInterval();
            }
        }

        private void ResetInterval()
        {
            ticksToHeal = Props.healingIntervalTicksRange.RandomInRange;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Heal permanent wound",
                    action = delegate
                    {
                        HediffComp_HealPermanentWounds.TryHealRandomPermanentWound(parent.pawn, parent.LabelCap);
                        ResetInterval();
                    }
                };
            }
        }

        public override void CompExposeData()
        {

            base.CompExposeData();
            Scribe_Values.Look(ref ticksToHeal, "ticksToHeal", 0);
        }
    }
}

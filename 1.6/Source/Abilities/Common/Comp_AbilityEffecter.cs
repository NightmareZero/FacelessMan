using Verse;
using RimWorld;
using System;

namespace NzFaceLessManMod
{
    public class CompProperties_AbilityEffecter : CompProperties_AbilityEffect
    {
        public EffecterDef effecterDef;

        public int maintainForTicks = -1;

        public float scale = 1f;

        // 是否在目标身上施放特效 false则施放在施法者身上
        public bool toTarget = true;

        public CompProperties_AbilityEffecter()
        {
            compClass = typeof(CompAbilityEffect_Effecter);
        }
    }

    public class CompAbilityEffect_Effecter : CompAbilityEffect
    {
        public new CompProperties_AbilityEffecter Props => (CompProperties_AbilityEffecter)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            // 释放目标
            Thing targetThing = parent.pawn;
            if (Props.toTarget)
            {
                targetThing = target.Pawn;
                if (targetThing == null)
                {
                    targetThing = target.Thing;
                }
            }

            Effecter effecter;
            if (targetThing == null)
            {
                effecter = Props.effecterDef.Spawn(target.Cell, parent.pawn.Map, Props.scale);
            }
            else
            {
                effecter = Props.effecterDef.Spawn(targetThing, parent.pawn.Map, Props.scale);
            }

            if (Props.maintainForTicks > 0)
            {
                parent.AddEffecterToMaintain(effecter, target.Cell, Props.maintainForTicks);
            }
            else
            {
                effecter.Cleanup();
            }
        }
    }
}
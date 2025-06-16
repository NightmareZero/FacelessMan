using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{ 
    public class CompAbilityFixWorstHealthCondition : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var pawn = target.Pawn;

            if (HealthUtility.TryGetWorstHealthCondition(pawn, out var hediff, out var part))
            {
                HealthUtility.FixWorstHealthCondition(pawn);
                Messages.Message(hediff.LabelCap + " has been treated.", pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return target.Pawn != null && HealthUtility.TryGetWorstHealthCondition(target.Pawn, out _, out _);
        }
    }
}
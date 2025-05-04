using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompProperties_MustNoResisting : CompProperties_AbilityEffect
    {
        public CompProperties_MustNoResisting()
        {
            this.compClass = typeof(CompAbility_MustNoResisting);
        }
    }

    public class CompAbility_MustNoResisting : CompAbilityEffect
    {
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }

            if (pawn.Faction != parent.pawn.Faction && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }

            if (pawn.IsWildMan() && !pawn.IsPrisonerOfColony && !pawn.Downed)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }

                return false;
            }

            if (ModsConfig.AnomalyActive && pawn.IsMutant)
            {
                return false;
            }

            return true;
        }

    }


}
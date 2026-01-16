using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProperties_GhoulControl : CompProperties_AbilityEffect
    {
        public CompProperties_GhoulControl()
        {
            compClass = typeof(CompAbilityEffect_GhoulControl);
        }
    }

    public class CompAbilityEffect_GhoulControl : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn targetPawn = target.Pawn;
            
            if (targetPawn == null)
            {
                Log.Error("GhoulControl: targetPawn is null");
                parent.ResetCooldown();
                return;
            }

            if (!IsGhoul(targetPawn))
            {
                Messages.Message("nzflm.ghoulcontrol.not_ghoul".Translate(), MessageTypeDefOf.RejectInput);
                parent.ResetCooldown();
                return;
            }

            List<HediffDef> availableMutations = GetAvailableMutations(targetPawn);
            
            if (availableMutations.Count == 0)
            {
                Messages.Message("nzflm.ghoulcontrol.no_available_mutations".Translate(), MessageTypeDefOf.RejectInput);
                parent.ResetCooldown();
                return;
            }

            HediffDef selectedMutation = availableMutations.RandomElement();
            InstallMutation(targetPawn, selectedMutation);
        }

        private bool IsGhoul(Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(AnomalyDefsOf.Ghoul) != null;
        }

        private List<HediffDef> GetAvailableMutations(Pawn pawn)
        {
            List<HediffDef> available = new List<HediffDef>();
            
            if (CanInstallMutation(pawn, AnomalyDefsOf.NzFlm_EnhancedAdrenalHeart))
            {
                available.Add(AnomalyDefsOf.NzFlm_EnhancedAdrenalHeart);
            }
            
            if (CanInstallMutation(pawn, AnomalyDefsOf.NzFlm_EnhancedAcidStomach))
            {
                available.Add(AnomalyDefsOf.NzFlm_EnhancedAcidStomach);
            }
            
            if (CanInstallMutation(pawn, AnomalyDefsOf.NzFlm_SteelBloodInjectorLiver))
            {
                available.Add(AnomalyDefsOf.NzFlm_SteelBloodInjectorLiver);
            }
            
            return available;
        }

        private bool CanInstallMutation(Pawn pawn, HediffDef mutationDef)
        {
            if (pawn.health.hediffSet.HasHediff(mutationDef))
            {
                return false;
            }

            BodyPartDef targetPart = mutationDef.defaultInstallPart;
            if (targetPart == null)
            {
                return false;
            }

            BodyPartRecord bodyPart = pawn.health.hediffSet.GetNotMissingParts()
                .FirstOrDefault(p => p.def == targetPart);
            
            if (bodyPart == null)
            {
                return false;
            }

            return !HasBionicPart(pawn, bodyPart);
        }

        private bool HasBionicPart(Pawn pawn, BodyPartRecord bodyPart)
        {
            return pawn.health.hediffSet.hediffs
                .Any(h => h.Part == bodyPart && h.def.spawnThingOnRemoved != null);
        }

        private void InstallMutation(Pawn pawn, HediffDef mutationDef)
        {
            BodyPartDef targetPart = mutationDef.defaultInstallPart;
            BodyPartRecord bodyPart = pawn.health.hediffSet.GetNotMissingParts()
                .FirstOrDefault(p => p.def == targetPart);
            
            if (bodyPart != null)
            {
                pawn.health.AddHediff(mutationDef, bodyPart);
                Messages.Message("nzflm.ghoulcontrol.installed".Translate(pawn.LabelShort, mutationDef.label), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}

using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class CompAbilityReimplantXeno : CompAbilityEffect
    {


        public CompProperties_AbilityReimplantXeno Props
        {
            get
            {
                return (CompProperties_AbilityReimplantXeno)this.props;
            }
        }

        public override void Apply(LocalTargetInfo caster, LocalTargetInfo dest)
        {
            base.Apply(caster, dest);
            // Pawn casterPawn = caster.Pawn;
            Pawn targetPawn = dest.Pawn;
            if (targetPawn == null)
            {
                return;
            }
            ReimplantGermline(parent.pawn, targetPawn);
            FleckMaker.AttachedOverlay(targetPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
            if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(targetPawn))
            {
                // int max = InternalDefOf.VRE_EndogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
                // int max2 = InternalDefOf.VRE_EndogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
                // Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "VRE_LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
            }
        }

        public static void ReimplantGermline(Pawn caster, Pawn dest)
        {

            // QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));

            List<GeneDef> xenogenesToPreserve = new List<GeneDef>();
            foreach (Gene gene in dest.genes.Xenogenes)
            {
                xenogenesToPreserve.Add(gene.def);
            }

            // 移除全部基因
            if (dest.genes?.Endogenes?.Count > 0)
            {
                for (int num = dest.genes.Endogenes.Count - 1; num >= 0; num--)
                {
                    dest.genes.RemoveGene(dest.genes.Endogenes[num]);
                }
            }
            if (dest.genes?.Xenogenes?.Count > 0)
            {
                for (int num = dest.genes.Xenogenes.Count - 1; num >= 0; num--)
                {
                    dest.genes.RemoveGene(dest.genes.Xenogenes[num]);
                }
            }

            // TODO 
            dest.genes.SetXenotype(caster.genes.Xenotype);
            dest.genes.xenotypeName = caster.genes.xenotypeName;
            dest.genes.xenotypeName = caster.genes.xenotypeName;
            dest.genes.iconDef = caster.genes.iconDef;

            List<Gene> adjustedList = new List<Gene>();

            foreach (Gene endogene in caster.genes.Endogenes)
            {
                if (endogene.def.defName.Contains("VRE_Morphs"))
                {
                    if (endogene.Active)
                    {
                        adjustedList.Add(endogene);
                    }
                }
                else adjustedList.Add(endogene);
            }


            foreach (Gene endogene in adjustedList)
            {
                dest.genes.AddGene(endogene.def, xenogene: false);

            }

            foreach (GeneDef xenogene in xenogenesToPreserve)
            {


                dest.genes.AddGene(xenogene, xenogene: true);

            }
            if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
            {
                caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(dest));
            }
            dest.health.AddHediff(InternalDefOf.VRE_EndogerminationComa);
            ExtractGermline(caster);
            GeneUtility.UpdateXenogermReplication(dest);

        }

        public static void ExtractGermline(Pawn pawn, int overrideDurationTicks = -1)
        {
            pawn.health.AddHediff(InternalDefOf.VRE_EndogermLossShock);
            if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
            {
                pawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
            }
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
            if (overrideDurationTicks > 0)
            {
                hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
            }
            pawn.health.AddHediff(hediff);
        }
    }
}
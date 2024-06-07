using System;
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

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            return base.GetPreCastActions();
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
            // TODO
            // ReimplantGermline(parent.pawn, targetPawn);
            // FleckMaker.AttachedOverlay(targetPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
            // if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(targetPawn))
            // {
            // int max = InternalDefOf.VRE_EndogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
            // int max2 = InternalDefOf.VRE_EndogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
            // Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "VRE_LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
            // }
        }

        /// <summary>
        /// 从一个Pawn身上, 获取全部的 "异种类型" 基因, 以其defName为key
        /// </summary>
        public static Dictionary<string, XenotypeDef> GetGeneXenotypes(Pawn pawn)
        {
            Dictionary<string, XenotypeDef> geneXenotypes = new Dictionary<string, XenotypeDef>();
            // 遍历pawn的全部基因，判断是否为异种携带者基因，加入返回值
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (isGeneXenotype(gene.def))
                {
                    geneXenotypes.Add(gene.def.defName, pawn.genes.Xenotype);
                }
            }

            return geneXenotypes;
        }

        /// <summary>
        /// 判断一个基因是否为本mod创建的 "异种类型携带者"基因
        /// </summary>
        public static bool isGeneXenotype(GeneDef geneDef)
        {
            return geneDef.defName.Contains("VRE_");
        }

        public static void ReimplantXenotype(Pawn caster, Pawn dest, XenotypeDef xenotype)
        {
            // 清除旧基因
            CleanOldGenes(dest, xenotype);

        }

        /// <summary>
        /// 移除旧基因
        /// 根据xenoType的内源性移除谱系/异种基因
        /// </summary>
        public static void CleanOldGenes(Pawn target, XenotypeDef xenotype)
        {
            // 获取谱系基因/异种基因flag
            bool isEndotype = xenotype.inheritable;
            if (isEndotype) // 根据flag移除谱系/异种基因
            {
                for (int num = target.genes.Endogenes.Count - 1; num >= 0; num--)
                {
                    target.genes.RemoveGene(target.genes.Endogenes[num]);
                }
            }
            else
            {
                for (int num = target.genes.Xenogenes.Count - 1; num >= 0; num--)
                {
                    target.genes.RemoveGene(target.genes.Xenogenes[num]);
                }
            }
        }



        // public static void ReimplantGermline(Pawn caster, Pawn dest)
        // {

        //     // QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));

        //     List<GeneDef> xenogenesToPreserve = new List<GeneDef>();
        //     foreach (Gene gene in dest.genes.Xenogenes)
        //     {
        //         xenogenesToPreserve.Add(gene.def);
        //     }

        //     // 移除全部基因
        //     if (dest.genes?.Xenogenes?.Count > 0)


        //         // TODO 
        //         dest.genes.SetXenotype(caster.genes.Xenotype);
        //     dest.genes.xenotypeName = caster.genes.xenotypeName;
        //     dest.genes.xenotypeName = caster.genes.xenotypeName;
        //     dest.genes.iconDef = caster.genes.iconDef;

        //     List<Gene> adjustedList = new List<Gene>();

        //     foreach (Gene endogene in caster.genes.Endogenes)
        //     {
        //         if (endogene.def.defName.Contains("VRE_Morphs"))
        //         {
        //             if (endogene.Active)
        //             {
        //                 adjustedList.Add(endogene);
        //             }
        //         }
        //         else adjustedList.Add(endogene);
        //     }


        //     foreach (Gene endogene in adjustedList)
        //     {
        //         dest.genes.AddGene(endogene.def, xenogene: false);

        //     }

        //     foreach (GeneDef xenogene in xenogenesToPreserve)
        //     {


        //         dest.genes.AddGene(xenogene, xenogene: true);

        //     }
        //     if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
        //     {
        //         caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(dest));
        //     }
        //     dest.health.AddHediff(InternalDefOf.VRE_EndogerminationComa);
        //     ExtractGermline(caster);
        //     GeneUtility.UpdateXenogermReplication(dest);

        // }

        // public static void ExtractGermline(Pawn pawn, int overrideDurationTicks = -1)
        // {
        //     pawn.health.AddHediff(InternalDefOf.VRE_EndogermLossShock);
        //     if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
        //     {
        //         pawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
        //     }
        //     Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
        //     if (overrideDurationTicks > 0)
        //     {
        //         hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
        //     }
        //     pawn.health.AddHediff(hediff);
        // }
    }
}
using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    [StaticConstructorOnStartup]
    public class CompAbilityReimplantXeno : CompAbilityEffect
    {


        // public CompProperties_AbilityReimplantXeno Props
        // {
        //     get
        //     {
        //         return (CompProperties_AbilityReimplantXeno)this.props;
        //     }
        // }
        private new CompProperties_AbilityReimplantXeno Props => (CompProperties_AbilityReimplantXeno)props;

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            return base.GetPreCastActions();
        }

        public override void Apply(LocalTargetInfo caster, LocalTargetInfo dest)
        {
            base.Apply(caster, dest);
            Pawn casterPawn = caster.Pawn;
            Pawn targetPawn = dest.Pawn;
            if (targetPawn == null)
            {
                Log.Message("targetPawn is null");
                return;
            }

            List<FloatMenuOption> selectXenoMenu = new List<FloatMenuOption>();

            // 从施放者的基因中寻找可用的基因组
            Dictionary<string, XenotypeDef> xenoGenes = Utils.GetGeneXenotypes(casterPawn);

            // 绘制菜单
            foreach (var xeno_ in xenoGenes)
            {
                var xeno = xeno_;

                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    ReimplantXenotype(casterPawn, targetPawn, xeno.Value);
                });
                selectXenoMenu.Add(opt);
            }

            // 显示 FloatMenu
            Find.WindowStack.Add(new FloatMenu(selectXenoMenu));
        }

        public static void ReimplantXenotype(Pawn caster, Pawn target, XenotypeDef xenotype)
        {
            // 清除旧基因
            CleanOldGenes(target, xenotype);
            // 设置基因类别
            target.genes.SetXenotype(xenotype);
            target.genes.xenotypeName = xenotype.defName; // TODO 是否使用这个值存疑
            // TODO 根据targetXentype获取对应的icondef 
            // dest.genes.iconDef = targetXenotype.;

            // 设置新基因
            AddNewGenes(target, xenotype);

            // 添加基因不稳定hediff
            target.health.AddHediff(XmlDefs.Flm_GeneticInstability);
            // 添加基因不稳定hediff
            SetExtractGermline(caster);
            // 更新异种基因复制
            GeneUtility.UpdateXenogermReplication(target);


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

        /// <summary>
        /// 添加新基因
        /// 根据xenoType的内源性添加谱系/异种基因
        /// </summary>
        public static void AddNewGenes(Pawn target, XenotypeDef xenotype)
        {
            foreach (GeneDef gene in xenotype.AllGenes)
            {
                target.genes.AddGene(gene, xenogene: !xenotype.inheritable);
            }
        }

        /// <summary>
        /// 为一个Pawn添加异种基因复制hediff, 并设置基因丢失冲击hediff
        /// 如果Pawn已经处于基因丢失状态, 则直接死亡
        /// </summary>
        public static void SetExtractGermline(Pawn caster, int overrideDurationTicks = -1)
        {
            caster.health.AddHediff(XmlDefs.Flm_GeneLossShock);
            if (GeneUtility.PawnWouldDieFromReimplanting(caster))
            {
                caster.genes.SetXenotype(XenotypeDefOf.Baseliner);
            }
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, caster);
            if (overrideDurationTicks > 0)
            {
                hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
            }
            caster.health.AddHediff(hediff);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return base.Valid(target, throwMessages);
            }
            if (pawn.IsQuestLodger())
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (!parent.pawn.genes.Endogenes.Any())
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_MessagePawnHasNoGermline".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            
            // if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
            // {
            //     if (throwMessages)
            //     {
            //         Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
            //     }
            //     return false;
            // }
            return base.Valid(target, throwMessages);
        }


        // public static void ReimplantGermline(Pawn caster, Pawn dest, XenotypeDef targetXenotype)
        // {

        //     // QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));

        //     List<GeneDef> xenogenesToPreserve = new List<GeneDef>();
        //     foreach (Gene gene in dest.genes.Xenogenes)
        //     {
        //         xenogenesToPreserve.Add(gene.def);
        //     }

        //     // TODO ->

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
        //     dest.health.AddHediff(XmlDefs.Flm_GeneticInstability);
        //     SetExtractGermline(caster);
        //     GeneUtility.UpdateXenogermReplication(dest);

        // }
    }
}
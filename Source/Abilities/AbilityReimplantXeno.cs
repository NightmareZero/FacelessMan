using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

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
            Pawn casterPawn = caster.Pawn;
            Pawn targetPawn = dest.Pawn;
            if (targetPawn == null)
            {
                return;
            }
    
            List<FloatMenuOption> selectXenoMenu = new List<FloatMenuOption>();

            // 从施放者的基因中寻找可用的基因组
            Dictionary<string, XenotypeDef> xenoGenes = GetGeneXenotypes(casterPawn);

            // 绘制菜单
            foreach (var xeno in xenoGenes)
            {
                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    ReimplantXenotype(casterPawn, targetPawn, xeno.Value);
                });
                selectXenoMenu.Add(opt);
            }

            // 显示 FloatMenu
            Find.WindowStack.Add(new FloatMenu(selectXenoMenu));
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
            var genoXeno = geneDef.GetModExtension<GeneXenoModExtension>();
            if (genoXeno != null)
            {
                return false;
            }

            return true;
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
            // 获取谱系基因/异种基因flag
            bool isEndotype = xenotype.inheritable;
            if (isEndotype) // 根据flag移除谱系/异种基因
            {
                foreach (Gene gene in target.genes.Endogenes)
                {
                    target.genes.AddGene(gene.def, xenogene: false);
                }
            }
            else
            {
                foreach (Gene gene in target.genes.Xenogenes)
                {
                    target.genes.AddGene(gene.def, xenogene: true);
                }
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
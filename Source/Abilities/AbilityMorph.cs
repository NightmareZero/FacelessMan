using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    [StaticConstructorOnStartup]
    public class CompAbilityMorph : CompAbilityEffect
    {


        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            return base.GetPreCastActions();
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn casterPawn = parent.pawn;
            Pawn targetPawn = casterPawn;
            if (targetPawn == null)
            {
                Log.Message("targetPawn is null");
                return;
            }

            List<FloatMenuOption> selectXenoMenu = new List<FloatMenuOption>();

            // 从施放者的基因中寻找可用的基因组
            Dictionary<string, XenotypeDef> xenoGenes = Utils.GetGeneXenotypes(casterPawn);

            Log.Message("xenoGenes.Count: " + xenoGenes.Count);
            if (xenoGenes.Count == 0)
            {
                // 在左上角弹出消息
                Messages.Message("NoXenotype".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }
            // 绘制菜单
            foreach (var xeno_ in xenoGenes)
            {
                var xeno = xeno_;
                #if DEBUG
                Log.Message("flm: draw menu xeno.Key: " + xeno.Key);
                #endif
                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    MorphXenotype(targetPawn, xeno.Value);
                    base.Apply(target, dest);
                });
                selectXenoMenu.Add(opt);
            }

            // 显示 FloatMenu
            Find.WindowStack.Add(new FloatMenu(selectXenoMenu));
        }

        /// <summary>
        /// 从施放者身上, 移除一个基因, 并将其移植到目标身上
        /// </summary>
        public static void MorphXenotype(Pawn target, XenotypeDef xeno)
        {
            // 清除旧基因
            CleanOldXenotype(target);
            // 添加新基因
            AddNewXenotypeGenes(target, xeno);
            // 更新异种基因复制
            GeneUtility.UpdateXenogermReplication(target);
        }

        /// <summary>
        /// 添加新基因
        /// 根据xenoType的内源性添加谱系/异种基因
        /// </summary>
        public static void AddNewXenotypeGenes(Pawn target, XenotypeDef xenotype)
        {
#if DEBUG
            Log.Message("flm: Add xenogene: " + xenotype.label);
#endif

            foreach (GeneDef gene in xenotype.AllGenes)
            {
#if DEBUG
                Log.Message("flm: Add gene: " + gene.label);
#endif
                target.genes.AddGene(gene, xenogene: true);
            }
        }


        /// <summary>
        /// 移除旧的异种基因
        /// </summary>
        public static void CleanOldXenotype(Pawn target)
        {
            for (int num = target.genes.Xenogenes.Count - 1; num >= 0; num--)
            {
                target.genes.RemoveGene(target.genes.Xenogenes[num]);
            }
        }
    }
}
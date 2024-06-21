using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityMorph : CompAbilityEffect
    {
        public CompProperties_AbilityMorph Props
        {
            get
            {
                return (CompProperties_AbilityMorph)this.props;
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
            Dictionary<string, XenotypeDef> xenoGenes = Utils.GetGeneXenotypes(casterPawn);

            // 绘制菜单
            foreach (var xeno in xenoGenes)
            {
                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    MorphXenotype(targetPawn, xeno.Value);
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
            foreach (GeneDef gene in xenotype.AllGenes)
            {
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
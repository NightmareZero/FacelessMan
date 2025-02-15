using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityEvolve : CompAbilityEffect
    {

        private CustomXenotype customXenotype;

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            Find.WindowStack.Add(new Dialog_CreateXenotype1(0, onAccept: (xeno) =>
            {
                customXenotype = xeno;
            }));
            return base.GetPreCastActions();
        }

        public override bool GizmoDisabled(out string reason)
        {
            reason = "";
            if (parent.pawn.health.hediffSet.GetFirstHediffOfDef(XmlDefs.Flm_GeneticInstability) != null)
            {
                reason = XmlDefs.Flm_GeneticInstability.label;
                return true;
            }
            return false;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            // 获取技能施放者
            Pawn caster = parent.pawn;
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null)
            {
                Log.Message("targetPawn is null");
                return;
            }

            if (customXenotype == null)
            {
                Messages.Message("nzflm.no_xenotype_selected".Translate(), MessageTypeDefOf.RejectInput);
                this.parent.ResetCooldown();
                return;
            }

            CustomXenotype(caster, targetPawn, customXenotype);
        }

        public static void CustomXenotype(Pawn caster, Pawn target, CustomXenotype xenotype)
        {
            // 清除旧基因
            CleanOldGenes(target);

            target.genes.xenotypeName = xenotype.name;
            target.genes.iconDef = xenotype.iconDef;


            // 设置新基因
            AddNewGenes(target, xenotype);

            // 添加基因不稳定hediff
            target.health.AddHediff(XmlDefs.Flm_GeneticInstability);
            caster.health.AddHediff(XmlDefs.Flm_GeneLossDizzy);

            // 更新异种基因复制
            GeneUtility.UpdateXenogermReplication(target);
        }

        public static void CleanOldGenes(Pawn target)
        {
            for (int num = target.genes.Xenogenes.Count - 1; num >= 0; num--)
            {
                target.genes.RemoveGene(target.genes.Xenogenes[num]);
            }
        }

        /// <summary>
        /// 添加新基因
        /// 根据xenoType的内源性添加谱系/异种基因
        /// </summary>
        public static void AddNewGenes(Pawn target, CustomXenotype xenotype)
        {
            foreach (GeneDef gene in xenotype.genes)
            {
                target.genes.AddGene(gene, xenogene: !xenotype.inheritable);
            }
        }
    }
}
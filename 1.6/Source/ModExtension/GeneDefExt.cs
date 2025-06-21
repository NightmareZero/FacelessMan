using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneDefExt : DefModExtension
    {
        public List<GeneExtCompProp> comps;
        // 强制设置性别
        public bool forceFemale = false;
        // 强制设置性别
        public bool forceMale = false;
        // HediffDef(绑定的状态)
        public HediffDef hediffDef = null;
        public bool hediffRemove = true;

        public bool mustEndogene = false; // 必须是内源基因

        // 调整基因的位置
        private static bool CanAdd(Gene gene, Pawn pawn, GeneDefExt defExt)
        {
            if (defExt.mustEndogene)
            {
                if (pawn.genes.HasXenogene(gene.def))
                {
                    pawn.genes.RemoveGene(gene);
                    return false;
                }
            }
            return true;
        }


        public static void ApplyGeneAdded(GeneExt gene)
        {
            GeneDefExt defExt = gene.def.GetModExtension<GeneDefExt>();
            if (defExt == null)
            {
                return;
            }

            // 如果为仅内源基因，则移除，添加到內源中
            if (!CanAdd(gene, gene.pawn, defExt))
            {
                Log.Error("Gene " + gene.def.defName + " must be endogene.");
                gene.pawn.genes.AddGene(gene.def, false);
                return;
            }

            // 处理绑定状态
            if (defExt.hediffDef != null)
            {
                Hediff hediff = gene.pawn.health.GetOrAddHediff(defExt.hediffDef);
                if (hediff is IGeneChangeListener listener)
                {
                    listener.Notify_OnGeneChange(gene, -1); // 通知基因新增
                }
            }

            // 处理性别
            if (defExt.forceFemale)
            {
                gene.pawn.gender = Gender.Female;
                if (gene.pawn.story?.bodyType == BodyTypeDefOf.Male)
                {
                    gene.pawn.story.bodyType = BodyTypeDefOf.Female;
                }
            }
            else if (defExt.forceMale)
            {
                gene.pawn.gender = Gender.Male;
                if (gene.pawn.story?.bodyType == BodyTypeDefOf.Female)
                {
                    gene.pawn.story.bodyType = BodyTypeDefOf.Male;
                }

            }
        }

        public static void ApplyRemoveGeneModExt(GeneExt gene)
        {
            GeneDefExt defExt = gene.def.GetModExtension<GeneDefExt>();
            if (defExt == null)
            {
                return;
            }

            // 处理绑定状态
            if (defExt.hediffDef != null)
            {
                Hediff hediff = gene.pawn.health.hediffSet.GetFirstHediffOfDef(defExt.hediffDef);
                if (hediff is IGeneChangeListener listener)
                {
                    listener.Notify_OnGeneChange(gene, -1); // 通知基因移除
                }
                if (hediff != null && defExt.hediffRemove)
                {
                    gene.pawn.health.RemoveHediff(hediff); // 执行移除
                }
            }
        }
        
        public override IEnumerable<string> ConfigErrors()
        {
            base.ConfigErrors();
            if (comps != null)
            {
                foreach (var comp in comps)
                {
                    foreach (var error in comp.ConfigErrors(this))
                    {
                        yield return error;
                    }
                    comp.ResolveReferences(this);
                }
            }
        }
    }
}
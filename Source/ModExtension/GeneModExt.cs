using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneDefExt : DefModExtension
    {
        // 强制设置性别
        public bool forceFemale = false;
        // 强制设置性别
        public bool forceMale = false;
        // HediffDef(绑定的状态)
        public HediffDef hediffDef = null;
        public bool hediffRemove = true;


        public static void ApplyAddGeneDefExt(GeneExt gene)
        {
            GeneDefExt geneDefExt = gene.def.GetModExtension<GeneDefExt>();
            if (geneDefExt == null)
            {
                return;
            }

            // 处理绑定状态
            if (geneDefExt.hediffDef != null)
            {
                Hediff hediff = gene.pawn.health.GetOrAddHediff(geneDefExt.hediffDef);
                if (hediff is IGeneChangeListener listener)
                {
                    listener.Notify_OnGeneChange(gene, -1); // 通知基因新增
                }
            }            

            // 处理性别
            if (geneDefExt.forceFemale)
            {
                gene.pawn.gender = Gender.Female;
                if (gene.pawn.story?.bodyType == BodyTypeDefOf.Male)
                {
                    gene.pawn.story.bodyType = BodyTypeDefOf.Female;
                }
            }
            else if (geneDefExt.forceMale)
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
            GeneDefExt geneDefExt = gene.def.GetModExtension<GeneDefExt>();
            if (geneDefExt == null)
            {
                return;
            }

            // 处理绑定状态
            if (geneDefExt.hediffDef != null)
            {
                Hediff hediff = gene.pawn.health.hediffSet.GetFirstHediffOfDef(geneDefExt.hediffDef);
                if (hediff is IGeneChangeListener listener)
                {
                    listener.Notify_OnGeneChange(gene, -1); // 通知基因移除
                }
                if (hediff != null && geneDefExt.hediffRemove)
                {
                    gene.pawn.health.RemoveHediff(hediff); // 执行移除
                }
            }
        }
    }
}
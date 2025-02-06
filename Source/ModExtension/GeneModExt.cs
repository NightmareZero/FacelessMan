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

            // 处理绑定状态
            if (geneDefExt.hediffDef != null)
            {
                gene.pawn.health.AddHediff(geneDefExt.hediffDef);
            }
        }

        public static void applyRemoveGeneModExt(GeneExt gene)
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
                if (hediff != null && geneDefExt.hediffRemove)
                {
                    gene.pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }
}
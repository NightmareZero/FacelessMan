using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneModExt : DefModExtension
    {
        // 强制设置性别
        public bool forceFemale = false;
        // 强制设置性别
        public bool forceMale = false;
        // HediffDef(绑定的状态)
        public HediffDef hediffDef = null;
        public bool hediffRemove = true;


        public static void applyAddGeneModExt(GeneExt gene)
        {
            GeneModExt geneModExt = gene.def.GetModExtension<GeneModExt>();
            if (geneModExt == null)
            {
                return;
            }

            // 处理性别
            if (geneModExt.forceFemale)
            {
                gene.pawn.gender = Gender.Female;
                if (gene.pawn.story?.bodyType == BodyTypeDefOf.Male)
                {
                    gene.pawn.story.bodyType = BodyTypeDefOf.Female;
                }
            }
            else if (geneModExt.forceMale)
            {
                gene.pawn.gender = Gender.Male;
                if (gene.pawn.story?.bodyType == BodyTypeDefOf.Female)
                {
                    gene.pawn.story.bodyType = BodyTypeDefOf.Male;
                }

            }

            // 处理绑定状态
            if (geneModExt.hediffDef != null)
            {
                gene.pawn.health.AddHediff(geneModExt.hediffDef);
            }
        }

        public static void applyRemoveGeneModExt(GeneExt gene)
        {
            GeneModExt geneModExt = gene.def.GetModExtension<GeneModExt>();
            if (geneModExt == null)
            {
                return;
            }

            // 处理绑定状态
            if (geneModExt.hediffDef != null)
            {
                Hediff hediff = gene.pawn.health.hediffSet.GetFirstHediffOfDef(geneModExt.hediffDef);
                if (hediff != null && geneModExt.hediffRemove)
                {
                    gene.pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }
}
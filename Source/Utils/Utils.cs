using RimWorld;
using System.Collections.Generic;
using Verse;

namespace NzFaceLessManMod
{
    public static class Utils
    {

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
        
        public static bool HasActiveGene(this Pawn pawn, GeneDef geneDef)
        {
            if (pawn.genes is null) return false;
            return pawn.genes.GetGene(geneDef)?.Active ?? false;
        }

        public static bool XenotypeContainsGene(XenotypeDef xenotype, GeneDef geneDef)
        {
            foreach (GeneDef gene in xenotype.AllGenes)
            {
                if (gene == geneDef)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ContainsOfDef(List<Gene> genes, GeneDef def)
        {
            foreach (Gene gene in genes)
            {
                if (gene.def == def)
                {
                    return true;
                }
            }
            return false;

        }
        public static bool ContainsAnyRelationOfDef(Pawn pawn, PawnRelationDef relation)
        {
            foreach (DirectPawnRelation directPawnRelation in pawn.relations?.DirectRelations)
            {
                if (directPawnRelation?.def == relation)
                {
                    return true;
                }

            }
            return false;

        }
    }
}
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace NzFaceLessManMod
{
    public static partial class Utils
    {
        static Dictionary<string, XenotypeIconDef> xenotypeIconCache = new Dictionary<string, XenotypeIconDef>();

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
                    foreach (var geneXenotype in getGeneXenotype(gene.def))
                    {
                        geneXenotypes.SetOrAdd(geneXenotype.Value.label, geneXenotype.Value);
                    }
                }
            }


            return geneXenotypes;
        }

         public static List<EvolutionGeneDef> cachedAllCanChooseEvolutionGene = null;

        public static List<EvolutionGeneDef> AllCanChooseEvolutionGene
        {
            get
            {
                if (cachedAllCanChooseEvolutionGene == null)
                {
                    cachedAllCanChooseEvolutionGene = new List<EvolutionGeneDef>();


                    List<EvolutionGeneDef> genelinegenes = DefDatabase<EvolutionGeneDef>.AllDefs.Where(x => !x.cannotBeChosen).ToList();


                    foreach (var allDef in genelinegenes)
                    {
                        cachedAllCanChooseEvolutionGene.Add(allDef);
                    }
                    cachedAllCanChooseEvolutionGene.SortBy((EvolutionGeneDef x) => 0f - x.displayCategory.displayPriorityInXenotype, (EvolutionGeneDef x) => x.displayCategory.label, (EvolutionGeneDef x) => x.displayOrderInCategory);
                }
                return cachedAllCanChooseEvolutionGene;
            }
        }


        /// <summary>
        /// 从一个Pawn身上, 获取全部的 "内源异种类型" 基因, 以其defName为key
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static Dictionary<string, XenotypeDef> GetGeneEndotypes(Pawn pawn)
        {
            Dictionary<string, XenotypeDef> geneEndotypes = new Dictionary<string, XenotypeDef>();
            // 遍历pawn的全部基因，判断是否为异种携带者基因，加入返回值
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (isGeneXenotype(gene.def))
                {
                    foreach (var geneXenotype in getGeneXenotype(gene.def))
                    {
                        if (geneXenotype.Value.inheritable)
                        {
                            geneEndotypes.SetOrAdd(geneXenotype.Value.label, geneXenotype.Value);
                        }
                    }
                }
            }
            return geneEndotypes;
        }

        /// <summary>
        /// 从XenotypeDef中获取其对应的图标
        /// </summary>
        public static XenotypeIconDef GetXenotypeIcon(XenotypeDef xenotype)
        {
            if (xenotypeIconCache.TryGetValue(xenotype.defName, out var icon))
            {
                return icon;
            }
            else
            {
                var iconDef = DefDatabase<XenotypeIconDef>.GetNamedSilentFail(xenotype.defName);
                if (iconDef != null)
                {
                    xenotypeIconCache[xenotype.defName] = iconDef;
                    return iconDef;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 判断一个基因是否为本mod创建的 "异种类型携带者"基因
        /// </summary>
        public static bool isGeneXenotype(GeneDef geneDef)
        {
            var genoXeno = geneDef.GetModExtension<GeneXenoModExt>();

            return genoXeno != null;
        }

        /// <summary>
        /// 判断一个基因是否为本mod创建的 "异种类型携带者"基因
        /// </summary>
        public static Dictionary<string, XenotypeDef> getGeneXenotype(GeneDef geneDef)
        {
            var genoXeno = geneDef.GetModExtension<GeneXenoModExt>();

            return genoXeno.GetContainGenes();
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
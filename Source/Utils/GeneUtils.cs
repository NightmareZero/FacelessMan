using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace NzFaceLessManMod
{
    public static partial class GeneUtil
    { 
        public static void ApplyEvolutionToPawn(this Evolution evolution,Pawn pawn)
        {
            // 先进行保存
            var evolutions = evolution.evolutions.ToList();
            var cache = evolutions.ToList();
            
            // 比对差异，进行修改
            pawn.genes.GenesListForReading.ForEach(x =>
            {
                // 删除所有不在selectedGenes的进化基因
                if (x.def is EvolutionGeneDef gene)
                {
                    if (!evolutions.Contains(gene))
                    {
                        pawn.genes.RemoveGene(x);
                        cache.Remove(gene);
                    }
                }
            });
            // 添加所有在selectedGenes的进化基因
            foreach (var gene in cache)
            {
                pawn.genes.AddGene(gene,false);
            }
          
            
        }
    }
}
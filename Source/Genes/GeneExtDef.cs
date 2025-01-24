using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtDef : GeneDef { 
        public new Type geneClass = typeof(GeneExt);
    }

    public class EvolutionGeneDef : GeneExtDef
    {
        // 取得该进化要求的点数
        public int evolution = 0;

        // 前置要求
        public List<EvolutionGeneDef> requireGene = new List<EvolutionGeneDef>();


    }
}
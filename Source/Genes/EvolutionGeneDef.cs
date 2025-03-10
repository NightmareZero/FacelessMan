using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{ 
    
    public class EvolutionGeneDef : GeneExtDef, IComparable<EvolutionGeneDef>
    {
        // 取得该进化要求的点数
        public int evolution = 0;

        // 不可自选
        public bool cannotBeChosen;

        // 前置要求
        public List<EvolutionGeneDef> requireGene = new List<EvolutionGeneDef>();

        // 排除要求
        public List<EvolutionGeneDef> excludeGene = new List<EvolutionGeneDef>();

        // 前置要求
        public GeneDef Prerequisite => prerequisite;

        // 冲突标签
        public List<string> ExclusionTags => exclusionTags;

        public int CompareTo(EvolutionGeneDef other)
        {
            return this.displayOrderInCategory.CompareTo(other.displayOrderInCategory);
        }
    }
}
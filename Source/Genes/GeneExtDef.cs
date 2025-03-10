using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtDef : GeneDef, IComparable<GeneExtDef>
    {
        public new Type geneClass = typeof(GeneExt);

        // 可作为变更监听器的Hediff
        public List<HediffDef> hediffListeners = new List<HediffDef>();

        public int CompareTo(GeneExtDef other)
        {
            return this.displayOrderInCategory.CompareTo(other.displayOrderInCategory);
        }
    }

}
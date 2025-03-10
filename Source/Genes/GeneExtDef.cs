using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtDef : GeneDef, IComparable<GeneExtDef>
    {
        public new Type geneClass = typeof(GeneExt);

        public int CompareTo(GeneExtDef other)
        {
            return this.displayOrderInCategory.CompareTo(other.displayOrderInCategory);
        }
    }

}
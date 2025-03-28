using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtDef : GeneDef, IComparable<GeneExtDef>
    {
        public new Type geneClass = typeof(GeneExt);

        public List<GeneExtCompProp> comps;

        // 可作为变更监听器的Hediff
        public List<HediffDef> hediffListeners = new List<HediffDef>();

        public int CompareTo(GeneExtDef other)
        {

            return this.displayOrderInCategory.CompareTo(other.displayOrderInCategory);
        }
        
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (comps != null)
            {
                foreach (var comp in comps)
                {
                    comp.ResolveReferences(this);
                }
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            base.ConfigErrors();
            if (comps == null)
            {
                yield return "comps is null";
            }
            else
            {
                foreach (var comp in comps)
                {
                    foreach (var error in comp.ConfigErrors(this))
                    {
                        yield return error;
                    }
                }
            }
        }
    }

}
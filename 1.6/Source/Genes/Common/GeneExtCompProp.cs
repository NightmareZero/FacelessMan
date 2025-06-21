using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtCompProp
    {
        [TranslationHandle]
        public Type compClass;

        public virtual void ResolveReferences(GeneDefExt parent)
        {
        }

        public virtual IEnumerable<string> ConfigErrors(GeneDefExt parentDef)
        {
            if (compClass == null)
            {
                yield return "compClass is null";
            }

            for (int i = 0; i < parentDef.comps.Count; i++)
            {
                if (parentDef.comps[i] != this && parentDef.comps[i].compClass == compClass)
                {
                    yield return "two comps with same compClass: " + compClass;
                }
            }
        }
    }
}
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

        public virtual void PostLoad()
        {
        }

        public virtual void ResolveReferences(HediffDef parent)
        {
        }
    }
}
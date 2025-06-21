using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NzFaceLessManMod
{
    public class Comp_ZeroSeverityOnRemove : HediffCompProperties
    {
        public Comp_ZeroSeverityOnRemove()
        {
            compClass = typeof(HediffComp_ZeroSeverityOnRemove);
        }
    }
    // public class HediffCompProperties_GizmoBtnRemoveThis : HediffCompProperties

    public class HediffComp_ZeroSeverityOnRemove : HediffComp
    {
        public Comp_ZeroSeverityOnRemove Props => (Comp_ZeroSeverityOnRemove)props;

        public override void CompPostPostRemoved()
        {
#if DEBUG
            Log.Message($"[NzFaceLessManMod] {parent} removed, severity set to 0f.");
#endif
            parent.Severity = 0f;
        }
    }
}
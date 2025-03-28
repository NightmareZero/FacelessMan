using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExtComp
    {
        public GeneExt parent;

        public GeneExtCompProp props;

        public Pawn Pawn => parent.pawn;

        public GeneExtDef Def => parent.Def;

        public virtual string Label => "";

        public virtual string LabelCap => Label.CapitalizeFirst();

        public virtual void CompPostMake()
        {
        }

        public virtual void CompPostTick()
        {
        }

        public virtual void CompExposeData()
        {

        }

        public virtual void CompPostPostAdd()
        {
        }

        public virtual void CompPostPostRemoved()
        {
        }

        public virtual void Notify_IngestedThing(Thing thing, int numTaken)
        {
        }

        public virtual void Notify_NewColony()
        {
        }

        public virtual void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {

        }

        public virtual void Reset()
        {
        }

        public virtual IEnumerable<Gizmo> CompGetGizmos()
        {
            return null;
        }

        // public virtual string CompDebugString()
        // {
        //     return null;
        // }
    }
}
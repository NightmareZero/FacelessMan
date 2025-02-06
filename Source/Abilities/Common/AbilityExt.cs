using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class AbilityExt : Ability
    {

        public virtual void BeforeStartCooldown(int ticks)
        {
        }

        public virtual void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group, int ticks)
        {
        }



        public AbilityExt() : base()
        {
        }

        public AbilityExt(Pawn pawn) : base(pawn)
        {
        }

        public AbilityExt(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept)
        {
        }

        public AbilityExt(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public AbilityExt(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {
        }
    }
}
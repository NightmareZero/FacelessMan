using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class EvAbility : AbilityExt
    {
        public override void BeforeStartCooldown(ref int ticks)
        {
            ticks = EvAbilityHelper.GetCooldown(pawn, ref ticks);
            base.BeforeStartCooldown(ref ticks);
        }

        public override void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group,ref int ticks)
        {
            ticks = EvAbilityHelper.GetCooldown(pawn, ref ticks);
            base.BeforeNotify_GroupStartedCooldown(group,ref ticks);
        }

        public EvAbility() : base()
        {
        }

        public EvAbility(Pawn pawn) : base(pawn)
        {
        }

        public EvAbility(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept)
        {
        }

        public EvAbility(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public EvAbility(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class EvAbilityPsycast : AbilityExtPsycast
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




        public EvAbilityPsycast(Pawn pawn) : base(pawn)
        {
        }

        public EvAbilityPsycast(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

    }
}
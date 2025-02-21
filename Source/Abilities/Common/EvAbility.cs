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
        public override void BeforeStartCooldown(int ticks)
        {

            base.BeforeStartCooldown(ticks);
        }

        public override void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group, int ticks)
        {
            base.BeforeNotify_GroupStartedCooldown(group, ticks);
        }
    }
}
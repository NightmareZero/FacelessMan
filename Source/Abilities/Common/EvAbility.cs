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
            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_SkillCooldownTime, true);
            float fTicks = ticks * cdVal;
            base.BeforeStartCooldown((int)fTicks);
        }

        public override void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group, int ticks)
        {
            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_SkillCooldownTime, true);
            float fTicks = ticks * cdVal;
            base.BeforeNotify_GroupStartedCooldown(group, (int)fTicks);
        }
    }
}
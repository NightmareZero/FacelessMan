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
            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_EvSkillCooldownTime, true);
            float fTicks = ticks * cdVal;
            base.BeforeStartCooldown((int)fTicks);
        }

        public override void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group, int ticks)
        {
            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_EvSkillCooldownTime, true);
            float fTicks = ticks * cdVal;
            base.BeforeNotify_GroupStartedCooldown(group, (int)fTicks);
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
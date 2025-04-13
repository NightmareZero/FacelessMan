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

            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_EvSkillCooldownTime, true, cacheStaleAfterTicks: 600);
            int fTicks = (int)(ticks * cdVal);
#if DEBUG
            Log.Message($"[NzFaceLessManMod] EvAbility.BeforeStartCooldown: {ticks} -> {fTicks}");
#endif
            ticks = fTicks;
            base.BeforeStartCooldown(ref ticks);
        }

        public override void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group,ref int ticks)
        {
            float cdVal = pawn.GetStatValue(XmlDefs.NzFlm_EvSkillCooldownTime, true, cacheStaleAfterTicks: 600);
            int fTicks = (int)(ticks * cdVal);
            ticks = fTicks;
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
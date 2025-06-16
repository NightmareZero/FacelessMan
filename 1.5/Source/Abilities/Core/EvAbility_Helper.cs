using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public static class EvAbilityHelper
    {
        public static int GetCooldown(Pawn pawn, ref int ticks)
        {
            float cdVal = pawn.GetStatValue(DefsOf.NzFlm_EvSkillCooldownTime, true, cacheStaleAfterTicks: 600);
            int fTicks = (int)(ticks * cdVal);
#if DEBUG
            Log.Message($"[NzFaceLessManMod] AdjustCooldown: {ticks} -> {fTicks}");
#endif
            return fTicks;
        }
    }
}
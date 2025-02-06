using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Verse.AI;
using RimWorld.Planet;

namespace NzFaceLessManMod
{
    /// <summary>
    /// 为Ability添加扩展
    /// </summary>
    [HarmonyPatch(typeof(Ability))]
    public static class AbilityExtCoolDownPatch
    {
        // 冷却时间预处理
        [HarmonyPatch(nameof(Ability.StartCooldown))]
        [HarmonyPrefix]
        public static void BeforeStartCooldown(Ability __instance, int ticks)
        {
            if (__instance is AbilityExt abilityExt)
            {
                abilityExt.BeforeStartCooldown(ticks);
            }
        }

        // 组冷却时间预处理
        [HarmonyPatch(nameof(Ability.Notify_GroupStartedCooldown))]
        [HarmonyPrefix]
        public static void BeforeNotify_GroupStartedCooldown(Ability __instance, AbilityGroupDef group, int ticks)
        {
            if (__instance is AbilityExt abilityExt)
            {
                abilityExt.BeforeNotify_GroupStartedCooldown(group, ticks);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;

namespace NzFaceLessManMod
{
    [HarmonyPatch(typeof(StatWorker_SuppressionFallRate), "GetValueUnfinalized")]
    public static class Patch_GetValueUnfinalized
    {
        [HarmonyPrefix]
        public static bool Prefix(StatRequest req, ref float __result)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null)
            {
                return true;
            }

            Need_Suppression need_Suppression;
            if (pawn.needs.TryGetNeed<Need_Suppression>(out need_Suppression) && need_Suppression != null)
            {
                // 如果能获取到有效的镇压需求，继续执行原方法
                return true;
            }

            // 如果没有镇压需求，返回默认值并跳过原方法
            __result = 0.5f;
            return false;
        }
    }

    [HarmonyPatch(typeof(StatWorker_SuppressionFallRate), "GetExplanationUnfinalized")]
    public static class Patch_GetExplanationUnfinalized
    {
        [HarmonyPostfix]
        public static void Postfix(StatRequest req, ref string __result)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null)
            {
                return;
            }

            Need_Suppression need_Suppression;
            if (!pawn.needs.TryGetNeed<Need_Suppression>(out need_Suppression) || need_Suppression == null)
            {
                // 如果没有镇压需求，移除包含CurrentSuppression的部分
                __result = __result.Replace(
                    string.Format("{0} (", "CurrentSuppression".Translate()),
                    ""
                );
            }
        }
    }

    [HarmonyPatch(typeof(StatWorker_SuppressionFallRate), "GetExplanationForTooltip")]
    public static class Patch_GetExplanationForTooltip
    {
        [HarmonyPrefix]
        public static bool Prefix(StatRequest req, ref string __result)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn == null)
            {
                return true;
            }

            Need_Suppression need_Suppression;
            if (pawn.needs.TryGetNeed<Need_Suppression>(out need_Suppression) && need_Suppression != null)
            {
                // 如果能获取到有效的镇压需求，继续执行原方法
                return true;
            }

            // 如果没有镇压需求，构造一个不包含镇压信息的工具提示
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SuppressionFallRate".Translate() + ": " + 0.2f.ToStringPercent());
            
            __result = stringBuilder.ToString();
            return false;
        }
    }
}
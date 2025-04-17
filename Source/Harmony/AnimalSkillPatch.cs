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
    [HarmonyPatch(typeof(Pawn_MindState))]
    [HarmonyPatch("StartFleeingBecauseOfPawnAction")]
    public static class Pawn_MindState_StartFleeingBecauseOfPawnAction_Patch
    {
        [HarmonyPrefix]
        public static bool CanStartFlee(Pawn_MindState __instance)
        {
            // 如果禁止逃跑则，不允许开始逃跑
            return !AnimalControl.NoEscape(__instance.pawn);
        }
    }

    [HarmonyPatch(typeof(JobGiver_Wander))]
    [HarmonyPatch("TryGiveJob")]
    public static class JobGiver_Wander_TryGiveJob_Patch
    {
        [HarmonyPrefix]
        public static bool PreventWanderingWhenDrafted(Pawn pawn, ref Job __result)
        {
            // 检查是否是可征召的动物，并且已被征召
            if (AnimalControl.CanDraft(pawn) && pawn.Drafted)
            {
                // 阻止游荡任务
                __result = JobMaker.MakeJob(JobDefOf.Wait, 600); // 让动物站立待命 600 tick
                return false;
            }

            // 允许原方法继续执行
            return true;
        }
    }


    [HarmonyPatch(typeof(PawnUtility))]
    [HarmonyPatch(nameof(PawnUtility.IsFighting))]
    public static class PawnUtility_IsFighting_Patch
    {
        [HarmonyPostfix]
        public static void DontFlee(Pawn pawn, ref bool __result)
        {
            if (pawn != null && AnimalControl.NoEscape(pawn) && pawn.CurJob != null) { __result = true; }
        }
    }

    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch(nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
    public static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
    {
        [HarmonyPostfix]
        public static void InitController(Pawn pawn)
        {
            if (pawn?.Faction?.IsPlayer == true && AnimalControl.CanDraft(pawn))
            {
                if (pawn.drafter is null)
                {
                    pawn.drafter = new Pawn_DraftController(pawn);
                }
                if (pawn.equipment is null)
                {
                    pawn.equipment = new Pawn_EquipmentTracker(pawn);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.WorkTypeIsDisabled))]
    public static class Pawn_WorkTypeIsDisabled_Patch
    {
        [HarmonyPostfix]
        public static void AnimalNoDoctor(WorkTypeDef w, Pawn __instance, ref bool __result)
        {
            if (w == WorkTypeDefOf.Doctor && AnimalControl.CanDraft(__instance))
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.IsColonistPlayerControlled), MethodType.Getter)]
    public static class Pawn_IsColonistPlayerControlled_Patch
    {
        [HarmonyPostfix]
        public static void AnimalCanCtrl(Pawn __instance, ref bool __result)
        {
            if (AnimalControl.CanDraft(__instance))
            {
                __result = __instance.Spawned && __instance.HostFaction == null && __instance.Faction == Faction.OfPlayer;
            }
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("CanTakeOrder")]
    public static class FloatMenuMakerMap_CanTakeOrder_Patch
    {
        [HarmonyPostfix]
        public static void AnimalAllowsFloatMenu(Pawn pawn, ref bool __result)

        {
            if (pawn.Faction?.IsPlayer == true &&  AnimalControl.CanDraft(pawn))
            {
                __result = true;
            }

        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddJobGiverWorkOrders")]
    public static class FloatMenuMakerMap_AddJobGiverWorkOrders_Patch
    {
        [HarmonyPrefix]
        public static bool AnimalNoWorkOrder(Pawn pawn)

        {

            if (AnimalControl.CanDraft(pawn))
            {
                return false;
            }
            return true;

        }
    }
    
    [HarmonyPatch(typeof(SchoolUtility))]
    [HarmonyPatch("CanTeachNow")]

    public static class SchoolUtility_CanTeachNow_Patch
    {
        [HarmonyPrefix]
        public static bool AnimalNoTeaching(Pawn teacher)

        {
            if (AnimalControl.CanDraft(teacher))
            {
                return false;

            }
            else return true;
        }
    }
}
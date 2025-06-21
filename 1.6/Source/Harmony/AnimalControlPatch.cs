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

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class Animal_Gizmos_Patch
    {
        static string toggleDesc = "CommandToggleDraftDesc".Translate().ToString();

        public static IEnumerable<Gizmo> AddGizmoButton(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            var pawn = __instance;
            var gizmos = __result;

            bool isAnimalAndDraftable = AnimalControl.CanDraftAndCtrl(pawn);
            bool hasDraftButtonAlready = false;
            foreach (var g in gizmos)
            {
                if (g is Command_Toggle command && command.defaultDesc == toggleDesc)
                {
                    hasDraftButtonAlready = true;
                }
                yield return g;
            }

            if (!hasDraftButtonAlready && isAnimalAndDraftable && pawn.drafter != null)
            {

                Command_Toggle drafting_command = new Command_Toggle();
                drafting_command.toggleAction = delegate
                {
                    pawn.drafter.Drafted = !pawn.drafter.Drafted;
                };
                drafting_command.isActive = () => pawn.drafter.Drafted;
                drafting_command.defaultLabel = (pawn.drafter.Drafted ? "CommandUndraftLabel" : "CommandDraftLabel").Translate();
                drafting_command.hotKey = KeyBindingDefOf.Command_ColonistDraft;
                drafting_command.defaultDesc = "CommandToggleDraftDesc".Translate();
                drafting_command.icon = ContentFinder<Texture2D>.Get("ui/commands/Draft", true);
                drafting_command.turnOnSound = SoundDefOf.DraftOn;
                drafting_command.groupKey = 81729172;
                drafting_command.turnOffSound = SoundDefOf.DraftOff;
                yield return drafting_command;
            }

            if (pawn.abilities != null && !isAnimalAndDraftable && AnimalControl.CanUseAbility(pawn))
            {
                foreach (Ability a in pawn.abilities.AllAbilitiesForReading)
                {

                    bool showGizmo = (pawn.Drafted || a.def.displayGizmoWhileUndrafted) && a.GizmosVisible();
                    if (!showGizmo)
                    {
                        continue;
                    }

                    foreach (Command gizmo in a.GetGizmos())
                    {
                        Command_Ability command_Ability;
                        if ((command_Ability = gizmo as Command_Ability) != null)
                        {
                            command_Ability.devGizmo = !showGizmo && DebugSettings.ShowDevGizmos;
                        }
                        yield return gizmo;
                    }


                    foreach (Gizmo item in a.GetGizmosExtra())
                    {
                        yield return item;
                    }
                }
            }

        }
    }

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
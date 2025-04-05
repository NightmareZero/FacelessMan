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
        public static bool DontFlee(Pawn_MindState __instance)

        {

            bool flagDoesCReatureNotFlee = AnimalCaches.nofleeing_animals.Contains(__instance.pawn);

            if (flagDoesCReatureNotFlee)
            {

                return false;
            }
            else return true;


        }
    }


    [HarmonyPatch(typeof(PawnUtility))]
    [HarmonyPatch("IsFighting")]
    public static class PawnUtility_IsFighting_Patch
    {
        [HarmonyPostfix]
        public static void DontFlee(Pawn pawn, ref bool __result)

        {
            if (pawn != null && AnimalCaches.nofleeing_animals.Contains(pawn) && pawn.CurJob != null) { __result = true; }



        }
    }

    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch("AddAndRemoveDynamicComponents")]
    public static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
    {
        [HarmonyPostfix]
        public static void AddDraftability(Pawn pawn)
        {
            //These two flags detect if the creature is part of the colony and if it has the custom class
            bool flagIsCreatureMine = pawn.Faction != null && pawn.Faction.IsPlayer;
            bool flagIsCreatureDraftable = AnimalCaches.draftable_animals.Contains(pawn);


            if (flagIsCreatureMine && flagIsCreatureDraftable)
            {
                //Log.Message("Patching "+ pawn.kindDef.ToString() + " with a draft controller and equipment tracker");
                //If everything goes well, add drafter and equipment to the pawn 
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
    [HarmonyPatch("WorkTypeIsDisabled")]
    public static class Pawn_WorkTypeIsDisabled_Patch
    {
        [HarmonyPostfix]
        public static void RemoveTendFromAnimals(WorkTypeDef w, Pawn __instance, ref bool __result)
        {
            if (w == WorkTypeDefOf.Doctor && AnimalCaches.draftable_animals.Contains(__instance)
                && __instance.RaceProps.IsMechanoid is false)
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
        public static void AddAnimalAsColonist(Pawn __instance, ref bool __result)
        {
            bool flagIsCreatureDraftable = AnimalCaches.draftable_animals.Contains(__instance);

            if (flagIsCreatureDraftable)
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
        public static void MakePawnControllable(Pawn pawn, ref bool __result)

        {
            bool flagIsCreatureMine = pawn.Faction != null && pawn.Faction?.IsPlayer == true;
            bool flagIsCreatureDraftable = AnimalCaches.draftable_animals.Contains(pawn);

            if (flagIsCreatureDraftable && flagIsCreatureMine)
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
        public static bool SkipIfAnimal(Pawn pawn)

        {

            if (AnimalCaches.draftable_animals.Contains(pawn))
            {
                return false;
            }
            return true;

        }
    }
}
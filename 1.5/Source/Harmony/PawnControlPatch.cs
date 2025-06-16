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
    [HarmonyPatch(typeof(Pawn_PathFollower))]
    [HarmonyPatch("CostToMoveIntoCell")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(IntVec3) })]
    public static class Pawn_MoveCost_Patch
    {

        private static float NoPass = 10240f;
        [HarmonyPostfix]
        public static void Patch(Pawn pawn, IntVec3 c, ref float __result)
        {
            var target = c;
            
            // 节约一些资源，防止报错
            if (pawn.Map == null || pawn.Dead || pawn.Downed || !PawnControl.CanMoveNoCost(pawn))
            {
                return;
            }

            TerrainDef terrainDef = pawn.Map.terrainGrid.TerrainAt(target);

            // 计算移动时间(直接计算，忽略减速)
            float cost = (target.x == pawn.Position.x || target.z == pawn.Position.z)
            ? pawn.TicksPerMoveCardinal : pawn.TicksPerMoveDiagonal;

            if (terrainDef == null || (terrainDef.passability == Traversability.Impassable))
            {
                cost = NoPass;
            }

            List<Thing> thingInRoad = pawn.Map.thingGrid.ThingsListAt(target);
            for (int i = 0; i < thingInRoad.Count; i++)
            {
                Thing thing = thingInRoad[i];
                if (thing.def.passability == Traversability.Impassable)
                {
                    cost = NoPass;
                }
            }

            __result = cost;
        }


    }
}
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace NzFaceLessManMod
{
    public static partial class Utils
    {

        /// <summary>
        /// 在影响区域内获取所有Pawn
        /// </summary>
        /// <param name="map"></param>
        /// <param name="affectedCells"></param>
        /// <returns></returns>
        public static List<Pawn> GetPawnsInAffectedArea(Map map, List<IntVec3> affectedCells)
        {
            List<Pawn> pawnsInAffectedArea = new List<Pawn>();
            foreach (IntVec3 cell in affectedCells)
            {
                foreach (Thing thing in cell.GetThingList(map))
                {
                    if (thing is Pawn pawn)
                    {
                        pawnsInAffectedArea.Add(pawn);
                    }
                }
            }
            return pawnsInAffectedArea;
        }

        /// <summary>
        /// 在影响区域内获取所有Pawn
        /// </summary>
        /// <param name="map">所在地图</param>
        /// <param name="center">中心格</param>
        /// <param name="radius">范围</param>
        /// <returns></returns>
        public static List<Pawn> GetPawnsInAffectedArea(Map map, IntVec3 center, float radius)
        {
            return GenRadial.RadialCellsAround(center, radius, true)
                .Where(cell => cell.InBounds(map)) // 检查 cell 是否在地图边界内
                .SelectMany(cell => cell.GetThingList(map).OfType<Pawn>())
                .ToList();
        }

        /// <summary>
        /// 在影响区域内获取所有Pawn, 并且对其执行action
        /// </summary>
        /// <param name="map">所在地图</param>
        /// <param name="center">中心格</param>
        /// <param name="radius">范围</param>
        /// <param name="action">对Pawn的操作</param>
        /// <returns></returns>
        public static int ApplyPawnInAffectedArea(Map map, IntVec3 center, float radius, System.Action<Pawn> action)
        {
            var count = 0;
            GenRadial.RadialCellsAround(center, radius, true)
                .Where(cell => cell.InBounds(map)) // 检查 cell 是否在地图边界内
                .SelectMany(cell => cell.GetThingList(map).OfType<Pawn>())
                .All(pawn =>
                {
                    try
                    {
                        action(pawn);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ApplyPawnInAffectedArea Error: {ex}");
                    }
                    return true;
                });

            return count;
        }

        /// <summary>
        /// 在影响区域内获取所有Pawn, 并且对其执行action
        /// </summary>
        /// <param name="map"></param>
        /// <param name="affectedCells"></param>
        /// <param name="action"></param>
        /// <returns>影响的Pawn数量</returns>
        public static int ApplyPawnInAffectedArea(Map map, List<IntVec3> affectedCells, System.Action<Pawn> action)
        {
            int count = 0;
            foreach (IntVec3 cell in affectedCells)
            {
                foreach (Thing thing in cell.GetThingList(map))
                {
                    if (thing is Pawn pawn)
                    {
                        try
                        {
                            action(pawn);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"ApplyPawnInAffectedArea Error: {ex}");
                        }
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 获取Pawn的所有物品, 包括携带物品, 穿戴装备, 装备武器
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static List<Thing> GetAllItems(this Pawn pawn)
        {
            List<Thing> allItems = new List<Thing>();

            // 获取Pawn的携带物品
            if (pawn.inventory != null)
            {
                allItems.AddRange(pawn.inventory.innerContainer.InnerListForReading);
            }

            // 获取Pawn的穿戴装备
            if (pawn.apparel != null)
            {
                allItems.AddRange(pawn.apparel.WornApparel);
            }

            // 获取Pawn的装备武器
            if (pawn.equipment != null)
            {
                allItems.AddRange(pawn.equipment.AllEquipmentListForReading);
            }

            return allItems;
        }

        /// <summary>
        /// 尝试获取最糟糕的健康状况(包括不可治愈的成瘾)
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="worstHealth"></param>
        /// <param name="worstBodyPart"></param>
        /// <returns></returns>
        public static bool TryGetWorstHealthCondition(Pawn pawn, out Hediff worstHealth, out BodyPartRecord worstBodyPart)
        {
            var get = HealthUtility.TryGetWorstHealthCondition(pawn, out worstHealth, out worstBodyPart);
            if (get)
            {
                return true;
            }

            // 找到身上一处成瘾
            var add = FindAllAddiction(pawn, null);
            if (add != null)
            {
                worstHealth = add;
                worstBodyPart = add.Part;
                return true;
            }


            return false;
        }

        /// <summary>
        /// 寻找所有成瘾(包括不可治愈的)
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        private static Hediff_Addiction FindAllAddiction(Pawn pawn, params HediffDef[] exclude)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i] is Hediff_Addiction addiction && addiction.Visible && (exclude == null || !exclude.Contains(hediffs[i].def)))
                {
                    return addiction;
                }
            }

            return null;
        }

        /// <summary>
        /// 尝试在地图边缘找到一个可达的入口
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="cell">入口</param>
        /// <returns></returns>
        public static bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }

        /// <summary>
        /// 尝试找到一个人类派系
        /// </summary>
        /// <param name="formerFaction"></param>
        /// <returns></returns>
        public static bool TryFindFormerFaction(out Faction formerFaction)
        {
            return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out formerFaction, tryMedievalOrBetter: false, allowDefeated: true);
        }
    }
}
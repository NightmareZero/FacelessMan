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
    /// 这个方法用于处理基因展示界面
    /// </summary>
    [HarmonyPatch(typeof(GeneUIUtility))]
    [HarmonyPatch("DrawSection")]
    public static class XenotypeUIPatch
    {
        static XenotypeUIPatch()
        {
#if DEBUG
            Log.Message("XenotypeUIPatch: loaded");
#endif
        }

        public static List<Gene> flmEvGenes = new List<Gene>();
        [HarmonyPrefix]
        public static void Prefix(List<Gene> ___xenogenes, List<Gene> ___endogenes, bool xeno, ref int count)
        {
            // #if DEBUG
            // Log.Message("flm: GeneUIUtility.DrawSection Prefix");
            // #endif
            if (xeno)
            {
                var flmEvGenesX = ___xenogenes.Where(x => x.def is EvolutionGeneDef).ToList();

                if (flmEvGenesX.Any())
                {
                    ___xenogenes.RemoveAll(x => x.def is EvolutionGeneDef);
                    count = ___xenogenes.Count;
                }

                flmEvGenes.AddRange(flmEvGenesX);
            } else
            {
                flmEvGenes = ___endogenes.Where(x => x.def is EvolutionGeneDef).ToList();
                if (flmEvGenes.Any())
                {
                    ___endogenes.RemoveAll(x => x.def is EvolutionGeneDef);
                    count = ___endogenes.Count;
                }
            }
        }
        [HarmonyPostfix]
        public static void Postfix(Rect rect, bool xeno, ref float curY, Rect containingRect, ref float ___xenogenesHeight, ref float ___endogenesHeight)
        {
            if (xeno && flmEvGenes.Any())
            {
                DrawSection(rect, flmEvGenes.Count, ref curY, ref ___xenogenesHeight, delegate (int i, Rect r)
                {
                    GeneUIUtility.DrawGene(flmEvGenes[i], r, GeneType.Xenogene);
                }, containingRect);
            } 
            // else if (!xeno && flmEvGenes.Any())
            // {
            //     DrawSection(rect, flmEvGenes.Count, ref curY, ref ___endogenesHeight, delegate (int i, Rect r)
            //     {
            //         GeneUIUtility.DrawGene(flmEvGenes[i], r, GeneType.Endogene);
            //     }, containingRect);
            // }
        }


        private static void DrawSection(Rect rect, int count, ref float curY, ref float sectionHeight,
            Action<int, Rect> drawer, Rect containingRect)
        {
            Widgets.Label(10f, ref curY, rect.width, "nzflm.evolutionGene".Translate(), "nzflm.evolutionGeneDesc".Translate());
            float num = curY;
            Rect rect2 = new Rect(rect.x, curY, rect.width, sectionHeight);
            Widgets.DrawMenuSection(rect2);
            float num2 = (rect.width - 12f - 630f - 36f) / 2f;
            curY += num2;
            int num3 = 0;
            int num4 = 0;
            for (int i = 0; i < count; i++)
            {
                if (num4 >= 6)
                {
                    num4 = 0;
                    num3++;
                }
                else if (i > 0)
                {
                    num4++;
                }
                Rect rect3 = new Rect(num2 + (float)num4 * 90f + (float)num4 * 6f, curY + (float)num3 * 90f + (float)num3 * 6f, 90f, 90f);
                if (containingRect.Overlaps(rect3))
                {
                    drawer(i, rect3);
                }
            }
            curY += (float)(num3 + 1) * 90f + (float)num3 * 6f + num2;
            if (Event.current.type == EventType.Layout)
            {
                sectionHeight = curY - num;
            }
        }

    }

    /// <summary>
    /// 这个方法用于处理基因编辑页面，将特定的基因隐藏起来
    /// </summary>
    [HarmonyPatch(typeof(GeneUtility), "GenesInOrder", MethodType.Getter)]
    public static class GeneUtility_GenesInOrder_Patch
    {
        private static bool logPrinted = false; // 添加静态布尔字段
        [HarmonyPriority(int.MinValue + 1)]
        public static void Postfix(ref List<GeneDef> __result)
        {
#if DEBUG
            if (!logPrinted) // 检查是否已经打印过日志
            {
                Log.Warning("flm: 无面人突变基因将不会被隐藏起来");
                logPrinted = true; // 设置为已打印
            }
#else      
            var window = Find.WindowStack.WindowOfType<GeneCreationDialogBase>();
            if (window != null)
            {
                __result = __result.Where(x => x is not EvolutionGeneDef).ToList();
            }
#endif
        }
    }
}
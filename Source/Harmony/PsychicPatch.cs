using HarmonyLib;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    [HarmonyPatch(typeof(Pawn_PsychicEntropyTracker), "PsyfocusFallPerDay", MethodType.Getter)]
    public static class PsyfocusFallPerDay_Patch
    {
        [HarmonyPostfix]
        public static void AdjustPsyfocusFall(ref float __result, Pawn ___pawn)
        {
            var pawn = ___pawn;


            // 检查是否满足条件： 包含特定Gene
            // TODO 之后采用性能更好的方案
            if (pawn?.genes.HasActiveGene(XmlDefs.Nzflm_Ev_Idealism) == true)
            {
                // 将精神力衰减翻转
                __result = -__result;
            }
        }
    }
}
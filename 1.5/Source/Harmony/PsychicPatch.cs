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
            // 未启用DLC
            if (!ModsConfig.RoyaltyActive)
            { 
                return;
            }

            var pawn = ___pawn;
            var mup = pawn.GetStatValue(DefsOf.NzFlm_PsychicRecovery, true, 600);
            if (mup == 0)
            {
                return;
            }

            // Log.Message($"[NzFaceLessManMod] PsyfocusFallPerDay: {__result} * {-mup}");
            __result = __result * __result < 0 ? mup : -mup;
        }
    }
}
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    [DefOf]
    public static class AnomalyDefsOf
    {
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef Ghoul;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef AdrenalHeart;
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef CorrosiveHeart;
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef MetalbloodHeart;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef GhoulFrenzy;
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef Metalblood;

        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef NzFlm_EnhancedAdrenalHeart;
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef NzFlm_EnhancedAcidStomach;
        [MayRequire("Ludeon.RimWorld.Anomaly")]
        public static HediffDef NzFlm_SteelBloodInjectorLiver;

        static AnomalyDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AnomalyDefsOf));
        }
    }
}

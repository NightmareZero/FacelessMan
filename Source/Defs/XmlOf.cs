using RimWorld;
using Verse;

namespace NzFaceLessManMod
{

    [DefOf]
    public static class XmlDefs
    {
        public static XenoGeneTemplateDef xenoGeneTemplateDef;

        // 注入目标 基因不稳定
        public static HediffDef Flm_GeneticInstability;

        // 注入者 基因丢失冲击
        public static HediffDef Flm_GeneLossShock;


        static XmlDefs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XmlDefs));
        }
    }
}
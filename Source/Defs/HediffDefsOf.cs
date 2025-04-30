using RimWorld;
using Verse;

namespace NzFaceLessManMod
{

    [DefOf]

    public static class HediffDefsOf
    {
        // 注入目标 基因不稳定
        public static HediffDef Flm_GeneticInstability;

        // 注入者 基因丢失冲击
        public static HediffDef Flm_GeneLossDizzy;

        // 进化(核心)基因
        public static HediffDef Flm_Evolution;

        // 支援信息素
        public static HediffDef NzFlm_He_SupportPheromone_Target;


        // 唯心主义
        [MayRequireRoyalty]
        public static EvolutionGeneDef Nzflm_Ev_Idealism;

        // 蠕虫攻击
        [MayRequireRoyalty]
        public static HediffDef NzFlm_He_MindWormAttack;
        // 蠕虫寄生
        [MayRequireRoyalty]
        public static HediffDef NzFlm_He_MindWormParasitic;
        // 蠕虫之主
        [MayRequireRoyalty]
        public static HediffDef NzFlm_He_MindWormLord;

        // 心智覆盖
        [MayRequireRoyalty]
        public static HediffDef NzFlm_He_MindWormCover;
        

        static HediffDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefsOf));
        }
    }
}
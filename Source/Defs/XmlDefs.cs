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
        public static HediffDef Flm_GeneLossDizzy;

        // 声音1 (暂时使用用作变身结束的声音)
        public static SoundDef FoamSpray_Resolve;

        // 超级携带者基因
        public static GeneDef Flm_GeneMaster;

        // 无面人基因
        public static XenotypeDef Flm_FacelessMan;
        
        // 内源变形基因
        public static GeneDef Flm_MorphsE;

        // 变身技能(异种)
        public static AbilityDef Flm_Morphing;

        public static HediffDef Flm_Evolution;

        // 支援信息素
        public static HediffDef NzFlm_He_SupportPheromone_Target;

        // 进化技能冷却时间
        public static StatDef NzFlm_EvSkillCooldownTime;

        // 承伤减值
        public static StatDef NzFlm_GlancingHitPoint;

        // 唯心主义
        public static EvolutionGeneDef Nzflm_Ev_Idealism;


        static XmlDefs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XmlDefs));
        }
    }
}
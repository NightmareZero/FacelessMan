using RimWorld;
using Verse;

namespace NzFaceLessManMod
{

    [DefOf]

    public static class DefsOf
    {
        public static XenoGeneTemplateDef xenoGeneTemplateDef;

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


        // 进化技能冷却时间
        public static StatDef NzFlm_EvSkillCooldownTime;

        // 承伤减值
        public static StatDef NzFlm_GlancingHitPoint;

        // 蠕虫_迷恋主人
        [MayRequireRoyalty]
        public static ThoughtDef NzFlm_Tk_ObsessedWithMaster;

        static DefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
        }
    }
}
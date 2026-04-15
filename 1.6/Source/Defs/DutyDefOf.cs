using Verse;

namespace FacelessMan
{
    [DefOf]
    public static class FacelessManDefOf
    {
        static FacelessManDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FacelessManDefOf));
        }
        
        // 自定义DutyDef
        public static DutyDef FacelessMan_Assault;
        public static DutyDef FacelessMan_Defend;
        public static DutyDef FacelessMan_Patrol;
        
        // 引用游戏内置的DutyDef
        public static DutyDef FleshbeastAssault = DutyDefOf.FleshbeastAssault;
    }
}
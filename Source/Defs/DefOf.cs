using RimWorld;

namespace NzFaceLessManMod
{

    [DefOf]
    public static class XmlDefs
    {
        public static XenoGeneTemplateDef xenoGeneTemplateDef;


        static XmlDefs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(XmlDefs));
        }
    }
}
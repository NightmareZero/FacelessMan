using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneXenoModExtension : DefModExtension
    {

        public XenotypeDef xenotypeDef = null;

        // 额外的基因包含
        public Dictionary<string, XenotypeDef> ContainXeno = new Dictionary<string, XenotypeDef>();
    }
}
using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneXenoModExt : DefModExtension
    {

        public XenotypeDef xenotypeDef = null;

        // 额外的基因包含
        public Dictionary<string, XenotypeDef> containXeno = new Dictionary<string, XenotypeDef>();

        public Dictionary<string,XenotypeDef> GetContainGenes() { 
            Dictionary<string, XenotypeDef> genes = new Dictionary<string, XenotypeDef>();
            if (xenotypeDef != null)
            {
                genes.SetOrAdd(xenotypeDef.defName,xenotypeDef);
            }
            foreach (var xeno in containXeno)
            {
                genes.SetOrAdd(xeno.Key,xeno.Value);
            }
            return genes;

        }
    }
}
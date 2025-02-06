using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class AbilityDefExt : DefModExtension { 
        // 允许生物质预准备(如果有目标基因，则额外添加一次充能)
        public bool acceptBiomassPreparation = false;
    }
}
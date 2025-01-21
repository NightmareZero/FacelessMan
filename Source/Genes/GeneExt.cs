using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExt : Gene
    {

        public override void PostAdd()
        {
            // 调用逻辑
            if (this.def.HasModExtension<GeneModExt>())
            {
                GeneModExt.applyAddGeneModExt(this);
            }

            base.PostAdd();
        }

        public override void PostRemove() { }
    }
}
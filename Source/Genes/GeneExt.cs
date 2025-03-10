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
            GeneDefExt gDef = this.def.GetModExtension<GeneDefExt>();
            if (gDef != null)
            {
                GeneDefExt.ApplyAddGeneDefExt(this); // 处理基因新增逻辑
            }

            base.PostAdd(); // renderer
        }

        public override void PostRemove()
        {

            GeneDefExt gDef = this.def.GetModExtension<GeneDefExt>();
            if (gDef != null)
            { 
                GeneDefExt.ApplyRemoveGeneModExt(this); // 处理基因移除逻辑
            }

            base.PostRemove(); // renderer
        }
    }
}
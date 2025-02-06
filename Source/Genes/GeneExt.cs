using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneExt : Gene
    {

        // TODO 这部分应该放在Hediff里面，用来保护Gene防止被移除
        private bool dirty = false;

        public override void Tick()
        {
            base.Tick();
            if (dirty)
            {
                OnDirty();
                dirty = false;
            }
        }

        public virtual void OnDirty() { 

        }

        public override void PostAdd()
        {
            // 调用逻辑
            if (this.def.HasModExtension<GeneDefExt>())
            {
                GeneDefExt.ApplyAddGeneDefExt(this);
            }

            base.PostAdd();
        }

        public override void PostRemove() { }
    }
}
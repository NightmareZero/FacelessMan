using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{
    public class ThingCompProperties_GlancingHit : CompProperties
    {
    }

    public class ThingComp_GlancingHit : ThingComp
    {
        private ThingCompProperties_GlancingHit Props => (ThingCompProperties_GlancingHit)props;

        public ThingComp_GlancingHit withParent(ThingWithComps parent)
        {
            this.parent = parent;
            return this;
        }
        

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(ref dinfo, out absorbed);

            // 没有加害者
            if (dinfo.Instigator == null)
            {
// #if DEBUG
//                 Log.Warning("ThingComp_GlancingHit_没有加害者");
// #endif
                return;
            }

            // 加害者是自身
            if (dinfo.Instigator == parent)
            {
// #if DEBUG
//                 Log.Warning("ThingComp_GlancingHit_加害者是自身");
// #endif                
                return;
            }

            // 获取偏斜值
            float gh = parent.GetStatValue(DefsOf.NzFlm_GlancingHitPoint, cacheStaleAfterTicks: 600);
            if (dinfo.Amount < gh)
            {
// #if DEBUG
//                 Log.Warning(string.Format("ThingComp_GlancingHit_偏斜值 {0} 大于伤害值 {1}", gh, dinfo.Amount));
// #endif
                absorbed = true; // 吸收伤害
                return;
            }
// #if DEBUG
//             Log.Warning(string.Format("ThingComp_GlancingHit_偏斜值 {0} 最初伤害值 {1} ", gh, dinfo.Amount));
// #endif
            // 计算偏斜值
            dinfo.SetAmount(dinfo.Amount - gh); // 减少伤害
        }
    }
}
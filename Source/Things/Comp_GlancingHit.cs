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
                return;
            }

            // 加害者是自身
            if (dinfo.Instigator == parent)
            {
                return;
            }

            // 获取偏斜值
            float gh = parent.GetStatValue(XmlDefs.NzFlm_GlancingHitPoint, cacheStaleAfterTicks: 600);
            if (dinfo.Amount < gh)
            {
                absorbed = true; // 吸收伤害
                return;
            }

            // 计算偏斜值
            dinfo.SetAmount(dinfo.Amount - gh); // 减少伤害
        }
    }
}
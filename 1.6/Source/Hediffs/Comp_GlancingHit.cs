using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Linq;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_GlancingHit : HediffCompProperties
    {
        public HediffCompProperties_GlancingHit()
        {
            compClass = typeof(HediffComp_GlancingHit);
        }
    }

    public class HediffComp_GlancingHit : HediffComp
    {
        private HediffCompProperties_GlancingHit Props => (HediffCompProperties_GlancingHit)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (!parent.pawn.HasComp<ThingComp_GlancingHit>())
            {
                parent.pawn.AllComps.Add(new ThingComp_GlancingHit().withParent(parent.pawn));
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            // 判断是否还有其他 HediffComp_GlancingHit
            if (!parent.pawn.health.hediffSet.GetHediffComps<HediffComp_GlancingHit>().Any(h => h != this))
            {
                // 如果没有其他 HediffComp_GlancingHit，移除 ThingComp_GlancingHit
                var comp = parent.pawn.GetComp<ThingComp_GlancingHit>();
                if (comp != null)
                {
                    parent.pawn.AllComps.Remove(comp);
                }
            }
        }
    }
}
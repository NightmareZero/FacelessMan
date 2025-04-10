using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProperties_Clotting : GeneExtCompProp
    {
        public int clotCheckInterval = 360;

        public FloatRange tendingQualityRange = new FloatRange(0.2f, 0.7f);

        public CompProperties_Clotting()
        {
            compClass = typeof(GeneExtComp_Clotting);
        }
    }

    public class GeneExtComp_Clotting : GeneExtComp
    {
        public CompProperties_Clotting Props => (CompProperties_Clotting)props;

        public override void CompPostTick()
        {
            base.CompPostTick();
            if (!parent.pawn.IsHashIntervalTick(Props.clotCheckInterval))
            {
                return;
            }

            // 检查流血速率
            if (parent.pawn.health.hediffSet.BleedRateTotal > 0f)
            {
                List<Hediff> hediffs = parent.pawn.health.hediffSet.hediffs;
                for (int num = hediffs.Count - 1; num >= 0; num--)
                {
                    if (hediffs[num].Bleeding)
                    {
                        hediffs[num].Tended(Props.tendingQualityRange.RandomInRange, Props.tendingQualityRange.TrueMax, 1);
                    }
                }
            }
        }
    }
}
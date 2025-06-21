using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{

    public class HediffCompProperties_GiveHediffsInRangeEx : HediffCompProperties
    {
        public float range;

        public TargetingParameters targetingParameters;

        public HediffDef hediff;

        public ThingDef mote;

        public bool hideMoteWhenNotDrafted;

        public float initialSeverity = 1f;

        public bool onlyPawnsInSameFaction = true;

        public HediffCompProperties_GiveHediffsInRangeEx()
        {
            compClass = typeof(HediffComp_GiveHediffsInRangeEx);
        }
    }

    public class HediffComp_GiveHediffsInRangeEx : HediffComp
    {
        // 自原版 HediffComp_GiveHediffsInRange 复制
        // 修改了对非 Humanlike 的支持
        private Mote mote;

        public HediffCompProperties_GiveHediffsInRangeEx Props => (HediffCompProperties_GiveHediffsInRangeEx)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!parent.pawn.Awake() || parent.pawn.health == null || parent.pawn.health.InPainShock || !parent.pawn.Spawned)
            {
                return;
            }

            if (!Props.hideMoteWhenNotDrafted || parent.pawn.Drafted)
            {
                if (Props.mote != null && (mote == null || mote.Destroyed))
                {
                    mote = MoteMaker.MakeAttachedOverlay(parent.pawn, Props.mote, Vector3.zero);
                }

                if (mote != null)
                {
                    mote.Maintain();
                }
            }

            IReadOnlyList<Pawn> readOnlyList = (!Props.onlyPawnsInSameFaction || parent.pawn.Faction == null) ? parent.pawn.Map.mapPawns.AllPawnsSpawned : parent.pawn.Map.mapPawns.SpawnedPawnsInFaction(parent.pawn.Faction);
            foreach (Pawn item in readOnlyList)
            {
                // 移除了只对 Humanlike 的限制
                if (item.Dead || item.health == null || item == parent.pawn || !(item.Position.DistanceTo(parent.pawn.Position) <= Props.range) || !Props.targetingParameters.CanTarget(item))
                {
                    continue;
                }

                Hediff hediff = item.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
                if (hediff == null)
                {
                    hediff = item.health.AddHediff(Props.hediff, item.health.hediffSet.GetBrain());
                    hediff.Severity = Props.initialSeverity;
                    HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
                    if (hediffComp_Link != null)
                    {
                        hediffComp_Link.drawConnection = true;
                        hediffComp_Link.other = parent.pawn;
                    }
                }

                HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears == null)
                {
                    Log.Error("HediffComp_GiveHediffsInRange has a hediff in props which does not have a HediffComp_Disappears");
                }
                else
                {
                    hediffComp_Disappears.ticksToDisappear = 5;
                }
            }
        }
    }
}
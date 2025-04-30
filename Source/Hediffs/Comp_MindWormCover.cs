using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace NzFaceLessManMod
{ 
    public class HediffCompProperties_MindWormCover : HediffCompProperties
    {
        public HediffCompProperties_MindWormCover()
        {
            compClass = typeof(HediffComp_MindWormCover);
        }

    }

    public class HediffComp_MindWormCover : HediffComp
    {

        private Pawn master = null; // 主人

        private HediffComp_MindWormSlave wormHediff = null; // 蠕虫核心Hediff

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            // 获取主人
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormCover: Failed to get master pawn for MindWormCover Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }

            // 获取蠕虫核心Hediff
            if (!parent.TryGetComp(out this.wormHediff))
            {
                Log.Error("MindWormCover: Failed to get wormHediff for MindWormCover Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }
        }
        
        public override void CompExposeData()
        {
            Scribe_References.Look(ref master, "master");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (!parent.TryGetComp(out wormHediff))
                {
                    Log.Error("MindWormCover: Failed to get wormHediff for MindWormCover Hediff on " + parent.pawn.Name.ToStringShort);
                    parent.pawn.health.RemoveHediff(parent);
                    Log.Warning("MindWormCover: Removed MindWormCover Hediff on " + parent.pawn.Name.ToStringShort + " due to missing wormHediff.");
                    return;
                }
            }
        }
    }
}
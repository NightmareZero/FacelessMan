using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_MindWormSlave : HediffCompProperties
    {
        public HediffCompProperties_MindWormSlave()
        {
            compClass = typeof(HediffComp_MindWormSlave);
        }

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            // 检查有没有Link
            if (!parentDef.HasComp(typeof(HediffCompProperties_Link)))
            {
                yield return "HediffDef " + parentDef.defName + " needs a Link comp to work properly.";
            }
        }
    }

    public partial class HediffComp_MindWormSlave : HediffComp
    {
        private HediffCompProperties_MindWormSlave Props => (HediffCompProperties_MindWormSlave)props;

        private Pawn master = null; // 主人

        private Hediff_MindWormMaster masterHediff = null; // 主人的Hediff

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormSlave: Failed to get master pawn for MindWormSlave Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }

            // 成熟的心灵蠕虫会将Slave添加到主人的列表中，以便控制
            this.masterHediff = (Hediff_MindWormMaster)master.health?.GetOrAddHediff(HediffDefsOf.NzFlm_He_MindWormLord);
            if (this.masterHediff == null)
            {
                Log.Error("MindWormSlave: Failed to get master Hediff for MindWormSlave Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }
            masterHediff?.AddWorm(this);

            // 添加想法
            Thought_MemorySocial thought = (Thought_MemorySocial)ThoughtMaker.MakeThought(DefsOf.NzFlm_Tk_ObsessedWithMaster);
            thought.otherPawn = master;
            parent.pawn?.needs?.mood?.thoughts?.memories?.TryGainMemory(thought, master);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormSlave: Failed to get master pawn for MindWormSlave Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }

            // 移除主人的列表中的自己
            this.masterHediff = (Hediff_MindWormMaster)master.health?.hediffSet.GetFirstHediffOfDef(HediffDefsOf.NzFlm_He_MindWormLord);
            masterHediff?.RemoveWorm(this);

            // 移除想法
            parent.pawn?.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(DefsOf.NzFlm_Tk_ObsessedWithMaster);

            // 移除心智覆盖
            if (parent.pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDefsOf.NzFlm_He_MindWormCover) is Hediff cover)
            {
                parent.pawn.health.RemoveHediff(cover);
            }
        }

        /// <summary>
        /// 失去主人时，移除自身，把后续的逻辑交给CompPostPostRemoved
        /// </summary>
        public void DoLostMaster()
        {
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormSlave: Failed to get master pawn for MindWormSlave Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }

            parent.pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).TryRandomElement(out var part);

            parent.pawn?.AddHediffExt(HediffDefOf.PsychicShock, caster: master, part, ticksToDisappear: 60000);
            // 移除自身
            parent.pawn?.health?.RemoveHediff(this.parent);
            
            Messages.Message("hzflm.slave_free_by_master_dead_wg".Translate(master.Name.Named("master"), parent.pawn.Name.Named("slave")), MessageTypeDefOf.NegativeEvent);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref master, "wormMaster", false);
            Scribe_References.Look(ref masterHediff, "masterHediff", false);
        }


    }
}
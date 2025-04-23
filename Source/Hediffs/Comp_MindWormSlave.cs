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

    public class HediffComp_MindWormSlave : HediffComp
    {
        private HediffCompProperties_MindWormSlave Props => (HediffCompProperties_MindWormSlave)props;

        private Pawn wormMaster = null; // 主人

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (wormMaster == null && !parent.TryGetCompLinkedOtherPawn(out wormMaster))
            {
                Log.Error("MindWormSlave: Failed to get master pawn for MindWormSlave Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }

            // TODO 成熟的心灵蠕虫会将Slave添加到主人的列表中，以便控制

            // 添加想法
            Thought_MemorySocial thought = (Thought_MemorySocial)ThoughtMaker.MakeThought(XmlDefs.NzFlm_Tk_ObsessedWithMaster);
            thought.otherPawn = wormMaster;
            parent.pawn?.needs?.mood?.thoughts?.memories?.TryGainMemory(thought, wormMaster);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            // TODO 从主人的列表中移除Slave

            // 移除想法
            parent.pawn?.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(XmlDefs.NzFlm_Tk_ObsessedWithMaster);
        }

        /// <summary>
        /// 失去主人时
        /// </summary>
        public void DoLostMaster()
        {
            // TODO 陷入昏迷

            // 移除自身
            parent.pawn?.health?.RemoveHediff(this.parent);
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref wormMaster, "wormMaster", false);
        }


    }
}
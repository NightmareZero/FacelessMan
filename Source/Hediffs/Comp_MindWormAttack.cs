using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_MindWormAttack : HediffCompProperties
    {
        public int mentalBreakInterval = 15000; // 精神崩溃间隔

        public float mentalBreakChanceHour = 0.2f; // 每小时精神崩溃几率

        public List<MentalStateDef> mentalBreaks = new List<MentalStateDef>(); // 精神崩溃类型

        public HediffCompProperties_MindWormAttack()
        {
            compClass = typeof(HediffComp_MindWormAttack);
        }

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }

            if (mentalBreakChanceHour < 0 || mentalBreakChanceHour > 1)
            {
                yield return "mentalBreakChanceHour must be between 0 and 1";
            }

            if (mentalBreaks.Count == 0)
            {
                yield return "mentalBreaks must contain at least one MentalStateDef";
            }
        }
    }

    public class HediffComp_MindWormAttack : HediffComp
    {
        private HediffCompProperties_MindWormAttack Props => (HediffCompProperties_MindWormAttack)props;

        private int ticksSinceLastMentalBreak = 0; // 上次精神崩溃的时间

        const int AnHour = 2500; // 一小时的Tick数

        private Pawn master = null; // 主人


        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormAttack: Failed to get master pawn for MindWormAttack Hediff on " + parent.pawn.Name.ToStringShort);
                return;
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
            {
                Log.Error("MindWormAttack: Failed to get master pawn on " + parent.pawn.Name.ToStringShort);
                return;
            }
            if (master.Dead)
            {
                Messages.Message("hzflm.slave_free_by_master_dead".Translate(master.Name.Named("master"), parent.pawn.Name.Named("slave")), MessageTypeDefOf.NegativeEvent);
                return;
            }

            Messages.Message("hzflm.slave_slaved_and_worm_growth".Translate(master.Name.Named("master"), parent.pawn.Name.Named("slave")), MessageTypeDefOf.NegativeEvent);
            // 添加成熟的心灵蠕虫Hediff
            parent.pawn.AddHediffExt(HediffDefsOf.NzFlm_He_MindWormParasitic, master,
            bodyPartRecord: this.parent?.pawn?.health.hediffSet?.GetBrain(), replaceExisting: true);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            // 已经可以触发下一次精神崩溃了
            if (Find.TickManager.TicksGame - ticksSinceLastMentalBreak > Props.mentalBreakInterval)
            {
                // 已经一小时了
                if (!parent.pawn.InMentalState && parent.pawn.IsHashIntervalTick(AnHour))
                {
                    // 计算精神崩溃的几率
                    if (Rand.Value < Props.mentalBreakChanceHour)
                    {
                        // 随机选择一个精神崩溃类型
                        MentalStateDef mentalBreak = Props.mentalBreaks.RandomElement();
                        // 触发精神崩溃
                        parent.pawn.mindState.mentalStateHandler.TryStartMentalState(mentalBreak, reason: parent.Label.Translate(), forceWake: true);
                        ticksSinceLastMentalBreak = Find.TickManager.TicksGame; // 更新上次精神崩溃的时间
                    }
                }
            }

            // 处理主人的逻辑
            if (parent.pawn.IsHashIntervalTick(1800))
            {
                if (master == null && !parent.TryGetCompLinkedOtherPawn(out master))
                {
                    Log.Error("MindWormAttack: Failed to get master pawn for MindWormAttack Hediff on " + parent.pawn.Name.ToStringShort);
                    parent.pawn?.health?.RemoveHediff(parent); // 移除this Hediff
                    return;
                }
                else if (master.Dead)
                {
                    parent.pawn?.health?.RemoveHediff(parent); // 移除this Hediff
                    Messages.Message("hzflm.slave_free_by_master_dead".Translate(master.Name.Named("master"), parent.pawn.Name.Named("slave")), MessageTypeDefOf.NegativeEvent);
                }
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksSinceLastMentalBreak, "ticksSinceLastMentalBreak", 0);
            Scribe_References.Look(ref master, "master", false);
        }
    }

}
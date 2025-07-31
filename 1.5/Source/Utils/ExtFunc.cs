using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace NzFaceLessManMod
{
    /// <summary>
    /// 所有提供 this 方法的
    /// </summary>
    public static partial class Utils
    {

        /// <summary>
        /// 尝试获取链接的另一个Pawn
        /// </summary>
        /// <param name="hediff"></param>
        /// <param name="otherPawn"></param>
        /// <returns></returns>
        public static bool TryGetCompLinkedOtherPawn(this HediffWithComps hediff, out Pawn otherPawn)
        {
            otherPawn = null;

            // 由于 'TryGetComp' 使用 is 判断，所以也可以获取到继承它的
            HediffComp_Link link = hediff.TryGetComp<HediffComp_Link>();
            if (link != null && link.OtherPawn != null)
            {
                otherPawn = link.OtherPawn;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 检查Hediff是否应该被移除
        /// 通过HediffComp_Disappears组件
        /// 如果没有HediffComp_Disappears组件，则返回false
        /// 如果有HediffComp_Disappears组件，但CompShouldRemove为false，则返回false
        /// 如果有HediffComp_Disappears组件，且CompShouldRemove为true，则返回true
        /// </summary>
        /// <param name="hediff"></param>
        /// <returns></returns>
        public static bool IsShouldRemovedByComp_Disappears(this Hediff hediff)
        {
            if (hediff == null)
            {
                return false;
            }

            HediffComp_Disappears disappearsComp = hediff.TryGetComp<HediffComp_Disappears>();
            if (disappearsComp == null || !disappearsComp.CompShouldRemove) // 修复：|| 改为 &&
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 给目标添加一个Hediff
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="hediffDef">Hediff定义</param>
        /// <param name="caster">施放者</param>
        /// <param name="bodyPartRecord">目标部位</param>
        /// <param name="bodyPartDef">目标部位定义，如果'bodyPartRecord'为null，则生效</param>
        /// <param name="toBodyIfNoPart">如果最后都找不到目标部位，是否添加到Body</param>
        /// <param name="replaceExisting">是否替换已有的</param>
        /// <param name="ticksToDisappear">替换持续时间</param>
        /// <param name="severity">替换严重度</param>
        public static void AddHediffExt(this Pawn target, HediffDef hediffDef, Pawn caster = null,
         BodyPartRecord bodyPartRecord = null, BodyPartDef bodyPartDef = null, bool toBodyIfNoPart = false, bool replaceExisting = true,
         int ticksToDisappear = -1, float severity = -1f)
        {
            if (target == null)
            {
                return;
            }

            // 替换已有的
            if (replaceExisting)
            {
                Hediff existHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if (existHediffOfDef != null)
                {
                    target.health.RemoveHediff(existHediffOfDef);
                }
            }

            // 尝试获取目标部位
            if (bodyPartRecord == null)
            {
                bodyPartRecord = target.health.hediffSet.GetBodyPartRecord(bodyPartDef);
                if (bodyPartRecord == null)
                {
                    bodyPartRecord = target.health.hediffSet.GetBodyPartRecord(hediffDef.defaultInstallPart);
                }
            }

            // 生成Hediff
            Hediff hediff = HediffMaker.MakeHediff(hediffDef, target, partRecord: bodyPartRecord);

            // 检查覆盖Hediff持续时间
            if (ticksToDisappear >= 0)
            {
                HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear = ticksToDisappear;
                }
            }

            // 检查覆盖Hediff严重度
            if (severity >= 0f)
            {
                hediff.Severity = severity;
            }

            // 注入Link连接
            // 由于 ‘TryGetComp’ 使用 is 判断，所以也可以获取到继承它的子类
            HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
            if (hediffComp_Link != null)
            {
                hediffComp_Link.other = caster;
                hediffComp_Link.drawConnection = target == caster;
            }

            target.health.AddHediff(hediff);
        }
    }
}
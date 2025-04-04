using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_MaxNeed : HediffCompProperties
    {
        // 最大睡眠值
        public float maxSleep = 0f;

        public float initSleep = 0f;

        // 最大饥饿值
        public float maxHunger = 0f;

        public float initHunger = 0f;

        public HediffCompProperties_MaxNeed()
        {
            compClass = typeof(HediffComp_MaxSleep);
        }
    }

    public class HediffComp_MaxSleep : HediffComp
    {
        private HediffCompProperties_MaxNeed Props => (HediffCompProperties_MaxNeed)props;


        public override void CompPostPostAdd(DamageInfo? dinfo)
        {

            if (parent.pawn?.needs?.rest.CurLevelPercentage < Props.initSleep)
            {
                // 设置初始睡眠值
                parent.pawn.needs.rest.CurLevelPercentage = Props.initSleep;
            }
            if (parent.pawn?.needs?.food.CurLevelPercentage < Props.initHunger)
            {
                // 设置初始饥饿值
                parent.pawn.needs.food.CurLevelPercentage = Props.initHunger;
            }
            // 初始化睡眠值和饥饿值
            if (Props.maxSleep > 0 && parent.pawn?.needs?.rest.CurLevelPercentage > Props.maxSleep)
            {
                // 设置最大睡眠值
                parent.pawn.needs.rest.CurLevelPercentage = Props.maxSleep;
            }
            if (Props.maxHunger > 0 && parent.pawn?.needs?.food.CurLevelPercentage > Props.maxHunger)
            {
                // 设置最大饥饿值
                parent.pawn.needs.food.CurLevelPercentage = Props.maxHunger;
            }
     

        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            // 每隔15秒检查一次
            if (Find.TickManager.TicksGame % 900 == 0)
            {
                if (Props.maxSleep > 0 && parent.pawn?.needs?.rest.CurLevelPercentage > Props.maxSleep)
                {
                    // 获取当前睡眠值
                    float currentSleep = parent.pawn.needs.rest.CurLevelPercentage;
                    // 如果当前睡眠值大于最大睡眠值，则设置为最大睡眠值
                    if (currentSleep > Props.maxSleep)
                    {
                        parent.pawn.needs.rest.CurLevelPercentage = Props.maxSleep;
                    }
                }

                if (Props.maxHunger > 0 && parent.pawn?.needs?.food.CurLevelPercentage > Props.maxHunger)
                {
                    // 获取当前饥饿值
                    float currentHunger = parent.pawn.needs.food.CurLevelPercentage;
                    // 如果当前饥饿值大于最大饥饿值，则设置为最大饥饿值
                    if (currentHunger > Props.maxHunger)
                    {
                        parent.pawn.needs.food.CurLevelPercentage = Props.maxHunger;
                    }
                }

            }
        }
    }
}
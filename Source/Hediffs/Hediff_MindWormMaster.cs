using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class Hediff_MindWormMaster : HediffWithComps
    {
        private HashSet<HediffComp_MindWormSlave> mindWorms = new HashSet<HediffComp_MindWormSlave>(); // 心灵蠕虫列表

        private HashSet<Pawn> slaves = new HashSet<Pawn>(); // 奴隶列表

        private bool dirtyCaches = true; // 是否需要更新缓存

        /// <summary>
        /// 是否可以使用心智操纵
        /// </summary>
        public bool CanMindCtrl = false;

        /// <summary>
        /// 是否可以使用心智塑形
        /// </summary>
        public bool CanMindShaping = false;

        /// <summary>
        /// 是否可以使用心智覆盖
        /// </summary>
        public bool CanMindCoverage = false;

        /// <summary>
        /// 是否可以使用任何心智能力
        /// </summary>        
        public bool CanMindAnything => CanMindCtrl || CanMindShaping || CanMindCoverage;

        public bool AddWorm(HediffComp_MindWormSlave worm)
        {
            if (worm == null || worm.parent == null || worm.parent.pawn == null)
            {
                Log.Error("MindWormMaster: addWorm failed, worm or its parent pawn is null.");
                return false;
            }

            if (mindWorms.Contains(worm))
            {
                Log.Error("MindWormMaster: addWorm failed, worm already exists in mindWorms list.");
                mindWorms.TryGetValue(worm, out var existingWorm);
                if (existingWorm.parent.pawn != null)
                {
                    Log.Error("MindWormMaster: Existing worm master is " + existingWorm.parent.pawn.Name.ToStringShort);
                }
                else
                {
                    mindWorms.Remove(existingWorm); // 移除已存在的蠕虫
                    mindWorms.Add(worm); // 添加新的蠕虫
                }
                return false;
            }
            mindWorms.Add(worm);
            if (worm.parent.pawn != null && !slaves.Contains(worm.parent.pawn))
            {
                slaves.Add(worm.parent.pawn); // 添加到奴隶列表
            }

            return true;
        }

        public bool RemoveWorm(HediffComp_MindWormSlave worm)
        {
            if (!mindWorms.Contains(worm))
            {
                Log.Error("MindWormMaster: delWorm failed, worm not found in mindWorms list.");
                return false;
            }

            mindWorms.Remove(worm);
            if (worm.parent.pawn != null)
            {
                slaves.Remove(worm.parent.pawn); // 从奴隶列表中移除
            }
            else
            {
                updateSlavesFromMindWorms(); // 更新奴隶列表
            }

            dirtyCaches = true; // 标记缓存需要更新
            UpdateCaches(); // 更新缓存

            return true;
        }

        private void updateSlavesFromMindWorms()
        {
            slaves.Clear();
            foreach (var worm in mindWorms.ToList())
            {
                if (worm != null && worm?.parent?.pawn != null)
                {
                    slaves.Add(worm.parent.pawn); // 添加到奴隶列表
                }
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();

            foreach (var worm in mindWorms.ToList())
            {
                worm?.parent?.pawn?.health?.hediffSet?.GetHediffComps<HediffComp_MindWormSlave>()?.ToList()
                .ForEach(x => x.DoLostMaster()); // 失去主人时，调用Slave的DoLostMaster方法
            }
        }

        public bool CanCtrl
        {
            get
            {
                // 检查是否可以控制人物
                return !pawn.DeadOrDowned && !pawn.InMentalState && !pawn.IsPrisoner;
            }
        }

        public override void PostTick()
        {
            base.PostTick();

            // 处理派系问题
            if (pawn?.Faction != null && !pawn.Faction.IsPlayer)
            {
                pawn?.SetFaction(Faction.OfPlayerSilentFail); // 设置为玩家派系
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mindWorms, "mindWorms", LookMode.Reference);
            // Scribe_Collections.Look(ref slaves, "slaves", LookMode.Reference);
            Scribe_Values.Look(ref CanMindCtrl, "canMindCtrl", false);
            Scribe_Values.Look(ref CanMindShaping, "canMindShaping", false);
            Scribe_Values.Look(ref CanMindCoverage, "canMindcoverage", false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                updateSlavesFromMindWorms();
                UpdateCaches();
            }
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="force"></param>
        public void UpdateCaches(bool force = false)
        {
            if (!dirtyCaches && !force)
            {
                return;
            }
            try
            {
                // 更新描述
                if (slaves != null && slaves.Count > 0)
                {
                    for (int i = 0; i < slaves.Count; i++)
                    {
                        cacheDescription += slaves.ElementAt(i).Name.ToStringShort + "\n ";
                    }
                }

            }
            finally
            {
                dirtyCaches = false;
            }
        }
        private string cacheDescription = null; // 描述

        public override string LabelBase
        {
            get
            {
                string baseLabel = base.LabelBase;
                if (slaves.Count > 0)
                {
                    return baseLabel + "nzflm.slaves_count".Translate(slaves.Count.ToString());
                }
                return baseLabel;
            }
        }

        public override string Description
        {
            get
            {
                string baseDescription = base.Description;
                if (slaves.Count > 0)
                {
                    UpdateCaches(); // 更新缓存
                    return baseDescription + "\n" + "nzflm.slaves_names".Translate(cacheDescription);
                }
                return baseDescription;

            }
        }
    }
}
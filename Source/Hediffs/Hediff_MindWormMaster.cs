using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class Hediff_MindWormMaster : HediffWithComps, ISetDirty
    {
        private HashSet<Pawn> slaves = new HashSet<Pawn>(); // 奴隶列表
        private HashSet<HediffComp_MindWormSlave> mindWorms = new HashSet<HediffComp_MindWormSlave>(); // 心灵蠕虫列表



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

            var slave = worm.parent.pawn;
            if (slaves.Contains(slave))
            {
                Log.Warning("MindWormMaster: addWorm failed, salve already exists in slave list.");
                return false;
            }


            slaves.Add(worm.parent.pawn); // 添加到奴隶列表
            mindWorms.Add(worm);

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
                updateMindWormsFromSlaves(); // 更新奴隶列表
            }

            dirtyCaches = true; // 标记缓存需要更新
            UpdateCaches(); // 更新缓存

            return true;
        }

        private void updateMindWormsFromSlaves()
        {
            mindWorms.Clear();
            foreach (var slave in slaves.ToList())
            {
                var ok = false;

                var wormHediff = (HediffWithComps)slave.health?.hediffSet?.GetFirstHediffOfDef(HediffDefsOf.NzFlm_He_MindWormParasitic);
                if (wormHediff != null)
                {
                    var worm = wormHediff?.GetComp<HediffComp_MindWormSlave>();
                    if (worm != null)
                    {
                        mindWorms.Add(worm); // 添加到奴隶列表
                        ok = true;
                    }
                }

                if (!ok)
                {
                    Log.Error("MindWormMaster: updateMindWormsFromSlaves failed, slave wormHediff got null.");
                    slaves.Remove(slave); // 移除奴隶
                }
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            doOnThisDead(); // 处理死亡
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            doOnThisDead(); // 处理死亡
            base.Notify_PawnDied(dinfo, culprit);
        }

        private void doOnThisDead()
        { 
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
            Scribe_Collections.Look(ref slaves, "slaves", LookMode.Reference);
            // Scribe_Collections.Look(ref mindWorms, "mindWorms", LookMode.Reference); // 这个不保存
            Scribe_Values.Look(ref CanMindCtrl, "canMindCtrl", false);
            Scribe_Values.Look(ref CanMindShaping, "canMindShaping", false);
            Scribe_Values.Look(ref CanMindCoverage, "canMindcoverage", false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                updateMindWormsFromSlaves();
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

                // 检查有没有基因
                CanMindCtrl = pawn.genes?.HasActiveGene(DefsOf.Nzflm_Ev_MindManipulation) ?? false;
                CanMindShaping = pawn.genes?.HasActiveGene(DefsOf.Nzflm_Ev_MindShaping) ?? false;
                CanMindCoverage = pawn.genes?.HasActiveGene(DefsOf.Nzflm_Ev_MindCover) ?? false;

            }
            finally
            {
                dirtyCaches = false;
            }
        }

        /// <summary>
        /// implement ISetDirty interface
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="condition"></param>
        /// <param name="obj"></param>
        public void SetDirty(DirtyTrigger trigger = DirtyTrigger.None, DirtyCondition condition = DirtyCondition.None, object obj = null)
        {
            dirtyCaches = true; // 设置为脏
            UpdateCaches(force: true);
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
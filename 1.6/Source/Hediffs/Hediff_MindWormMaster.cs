using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class Hediff_MindWormMaster : HediffWithComps, ISetDirty
    {
        private HashSet<Pawn> slaves = new HashSet<Pawn>(); // 奴隶列表
        private Dictionary<Pawn, HediffComp_MindWormSlave> mindWorms = new Dictionary<Pawn, HediffComp_MindWormSlave>(); // 心灵蠕虫字典

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
                Log.Warning("MindWormMaster: addWorm failed, slave already exists in slave list.");
                return false;
            }

            slaves.Add(slave); // 添加到奴隶列表
            mindWorms[slave] = worm; // 添加到字典

            return true;
        }

        public bool RemoveWorm(HediffComp_MindWormSlave worm)
        {
            if (worm == null || worm.parent?.pawn == null)
            {
                Log.Error("MindWormMaster: delWorm failed, worm or its parent pawn is null.");
                return false;
            }
            var slave = worm.parent.pawn;
            if (!mindWorms.ContainsKey(slave))
            {
                Log.Error("MindWormMaster: delWorm failed, worm not found in mindWorms dict.");
                return false;
            }

            mindWorms.Remove(slave);
            slaves.Remove(slave); // 从奴隶列表中移除

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
                        mindWorms[slave] = worm; // 添加到字典
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
            foreach (var worm in mindWorms.Values.ToList())
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

        public override IEnumerable<Gizmo> GetGizmos()
        {
            for (int i = 0; i < base.GetGizmos().Count(); i++)
            {
                yield return base.GetGizmos().ElementAt(i);
            }

            yield return new Command_Action
            {
                defaultLabel = "NzFLM.MindWormSlave_Gizmo".Translate(),
                defaultDesc = "NzFLM.MindWormSlave_GizmoDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Icons/Abilities/Psionic/MindManipulation"),
                action = delegate
                {
                    List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>();
                    this.AddMenuOptions(cmdMenu); // 添加菜单选项
                    // 弹出菜单
                    FloatMenu menu = new FloatMenu(cmdMenu)
                    {
                        vanishIfMouseDistant = false,
                        // 弹出时暂停游戏
                        forcePause = true,
                    };
                    Find.WindowStack.Add(menu);
                },
                hotKey = KeyBindingDefOf.Misc1,
            };

        }

        private void AddMenuOptions(List<FloatMenuOption> menu)
        {

            if (this.CanMindShaping)
            {
                menu.Add(new FloatMenuOption("nzflm.slave_remove_trait".Translate(), delegate
                {
                    this.SubMenu_RemoveTrait();
                }));
            }
            if (this.slaves != null && this.slaves.Count > 0)
            {
                menu.Add(new FloatMenuOption("nzflm.free_slaves".Translate(), delegate
                {
                    this.SubMenu_ManageSlave();
                }));
                menu.Add(new FloatMenuOption("nzflm.kill_slaves".Translate(), delegate
                {
                    this.SubMenu_ManageSlave(true);
                }));
            }
        }

        private void SubMenu_ManageSlave(bool kill = false)
        {
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>();
            if (this.slaves != null && this.slaves.Count > 0)
            {
                //
                // 遍历所有的奴隶
                for (int i = 0; i < slaves.Count; i++)
                {
                    var thisSlave = slaves.ElementAt(i);
                    // 添加奴隶到菜单
                    cmdMenu.Add(new FloatMenuOption(thisSlave.Name.ToStringShort, delegate
                    {
                        if (kill)
                        {
                            thisSlave?.Kill(null, null);
                            Messages.Message("hzflm.slave_kill_by_master".Translate(pawn.Named("master"), thisSlave.Named("slave")),
                                MessageTypeDefOf.NegativeHealthEvent);
                        }
                        else
                        {
                            var got = false;

                            var wormHediff = (HediffWithComps)thisSlave.health?.hediffSet?.GetFirstHediffOfDef(HediffDefsOf.NzFlm_He_MindWormParasitic);
                            if (wormHediff != null)
                            {
                                thisSlave?.health?.RemoveHediff(wormHediff); // 移除奴隶的心灵蠕虫(蠕虫会自动调用master的移除)
                                got = true;
                            }

                            if (!got)
                            {
                                Log.Error("MindWormMaster: updateMindWormsFromSlaves failed, slave wormHediff got null.");
                                slaves.Remove(thisSlave); // 移除奴隶
                            }
                            Messages.Message("hzflm.slave_free_by_master".Translate(pawn.Named("master"), thisSlave.Named("slave")),
                                MessageTypeDefOf.PositiveEvent);
                        }
                    }));
                }
            }

            else
            {
                return;
            }
            // 弹出菜单
            FloatMenu menu = new FloatMenu(cmdMenu)
            {
                vanishIfMouseDistant = false,
                // 弹出时暂停游戏
                forcePause = true,
            };
            Find.WindowStack.Add(menu);
        }

        private void SubMenu_RemoveTrait()
        {
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>();
            // 获取所有的特质
            List<Trait> traits = pawn?.story?.traits?.allTraits.ToList();
            if (traits != null && traits.Count > 0)
            {
                // 遍历所有的特质
                for (int i = 0; i < traits.Count; i++)
                {
                    // 如果是基因的特质，跳过
                    if (traits[i].sourceGene != null)
                    {
                        continue;
                    }
                    var thisTrait = traits[i];
                    // 添加特质到菜单
                    cmdMenu.Add(new FloatMenuOption(thisTrait.LabelCap, delegate
                    {
                        pawn?.story?.traits?.RemoveTrait(thisTrait);
                        // 添加过载
                        pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.3f,
                            pawn?.health?.hediffSet?.GetBrain());
                        // 播放音效
                        DefsOf.Psycast_Skip_Pulse.PlayOneShot(pawn);
                        Messages.Message("nzflm.slave_remove_trait".Translate(), MessageTypeDefOf.PositiveEvent);
                    }));
                }
            }
            else
            {
                return;
            }
            // 弹出菜单
            FloatMenu menu = new FloatMenu(cmdMenu)
            {
                vanishIfMouseDistant = false,
                // 弹出时暂停游戏
                forcePause = true,
            };
            Find.WindowStack.Add(menu);

        }
    }
}
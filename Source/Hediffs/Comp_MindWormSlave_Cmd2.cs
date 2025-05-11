using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Linq;

namespace NzFaceLessManMod
{
    // 主要处理Gizmo的显示
    public partial class HediffComp_MindWormSlave : HediffComp
    {


        private void AddShapingCmd(List<FloatMenuOption> menu)
        {
            // 抹除记忆(全部特质和记忆)
            menu.Add(new FloatMenuOption("nzflm.slave_remove_memory".Translate(), delegate
            {
                ShapingCmdRemoveMemory();

                // 播放音效
                DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                Messages.Message("nzflm.slave_remove_memory".Translate(), MessageTypeDefOf.PositiveEvent);
            }, MenuOptionPriority.Default, null, null, 0f, null, null));
            // 抽取知识 战斗(近战，远程，医疗)/智慧(智识，艺术，手工)/劳作(建筑，采矿，种植，烹饪)/交流(社交，驯兽)
            menu.Add(new FloatMenuOption("nzflm.slave_extract_knowledge".Translate(), delegate
            {
                SubMenu_ShapingCmd_ExtractKnowledge();
            }));
            // 传输知识 如上
            menu.Add(new FloatMenuOption("nzflm.slave_transfer_knowledge".Translate(), delegate
            {
                SubMenu_ShapingCmd_TransferKnowledge();
            }));
            // 记忆抽取(从目标的特质中选一条，加入自己的特质，不能超过6条)
            menu.Add(new FloatMenuOption("nzflm.slave_extract_trait".Translate(), delegate
            {
                if (master?.story?.traits?.allTraits?.Count < 6 && parent.pawn?.story?.traits?.allTraits?.Count > 0)
                {
                    SubMenu_ExtractTrait();

                }
            }));

            // 记忆传输(从自己的特质中选一条，不能超过6条)
            menu.Add(new FloatMenuOption("nzflm.slave_transfer_trait".Translate(), delegate
            {
                if (master?.story?.traits?.allTraits?.Count > 0 && parent.pawn?.story?.traits?.allTraits?.Count < 6)
                {
                    SubMenu_TransferTrait();
                }
            }));

        }

        private void SubMenu_TransferTrait()
        {
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>();
            // 获取所有的特质
            List<Trait> traits = master?.story?.traits?.allTraits.ToList();
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
                         var newTrait = new Trait(thisTrait.def, thisTrait.Degree, true);
                        // 添加特质到自己的特质列表
                        parent.pawn?.story?.traits?.GainTrait(newTrait);
                        // 添加过载
                        parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.5f,
                            master?.health?.hediffSet?.GetBrain());
                        // 播放音效
                        DefsOf.Psycast_Skip_Pulse.PlayOneShot(master);
                        Messages.Message("nzflm.slave_transfer_trait".Translate(), MessageTypeDefOf.PositiveEvent);
                    }));
                }
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

        private void SubMenu_ExtractTrait()
        {
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>();
            // 获取所有的特质
            List<Trait> traits = parent.pawn?.story?.traits?.allTraits.ToList();
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
                        var newTrait = new Trait(thisTrait.def, thisTrait.Degree, true);
                        // 添加特质到主人的特质列表
                        master?.story?.traits?.GainTrait(newTrait);
                        // 添加过载
                        master?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                            parent.pawn?.health?.hediffSet?.GetBrain());
                        // 播放音效
                        DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                        Messages.Message("nzflm.slave_extract_trait".Translate(), MessageTypeDefOf.PositiveEvent);
                    }));
                }
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

        // 抽取知识
        private void SubMenu_ShapingCmd_ExtractKnowledge()
        {
            // 创建二级菜单
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>
            {
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Fight".Translate(), delegate
                {
                    // 战斗技艺
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Melee);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Shooting);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Medicine);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                        parent.pawn?.health?.hediffSet?.GetBrain());

                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(master);
                    Messages.Message("nzflm.worm_extract_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Wisdom".Translate(), delegate
                {
                    // 智慧技艺
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Intellectual);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Artistic);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Crafting);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                        parent.pawn?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(master);
                    Messages.Message("nzflm.worm_extract_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Work".Translate(), delegate
                {
                    // 劳作技艺
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Construction);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Mining);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Plants);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Cooking);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.9f,
                        parent.pawn?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(master);
                    Messages.Message("nzflm.worm_extract_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Communicate".Translate(), delegate
                {
                    // 交流技艺
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Social);
                    doTransferKnowledge(parent.pawn, master, 0.25f, SkillDefOf.Animals);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                        parent.pawn?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(master);
                    Messages.Message("nzflm.worm_extract_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                })
            };
            FloatMenu menu = new FloatMenu(cmdMenu)
            {
                vanishIfMouseDistant = false,
                // 弹出时暂停游戏
                forcePause = true,
            };
            Find.WindowStack.Add(menu);
        }

        /// <summary>
        /// 从'master'传输知识到'thisPawn'
        /// </summary>
        private void SubMenu_ShapingCmd_TransferKnowledge()
        {
            // 创建二级菜单
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>
            {
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Fight".Translate(), delegate
                {
                    // 战斗技艺
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Melee);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Shooting);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Medicine);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                        master?.health?.hediffSet?.GetBrain());

                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                    Messages.Message("nzflm.worm_transfer_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Wisdom".Translate(), delegate
                {
                    // 智慧技艺
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Intellectual);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Artistic);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Crafting);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.6f,
                        master?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                    Messages.Message("nzflm.worm_transfer_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Work".Translate(), delegate
                {
                    // 劳作技艺
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Construction);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Mining);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Plants);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Cooking);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.7f,
                        master?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                    Messages.Message("nzflm.worm_transfer_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Communicate".Translate(), delegate
                {
                    // 交流技艺
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Social);
                    doTransferKnowledge(master, parent.pawn, 0.5f, SkillDefOf.Animals);
                    // 添加过载
                    parent.pawn?.AddHediffSeverity(HediffDefsOf.NzFlm_He_MindWormOverload, 0.4f,
                        master?.health?.hediffSet?.GetBrain());
                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
                    Messages.Message("nzflm.worm_transfer_knowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                })
            };
            FloatMenu menu = new FloatMenu(cmdMenu)
            {
                vanishIfMouseDistant = false,
                // 弹出时暂停游戏
                forcePause = true,
            };
            Find.WindowStack.Add(menu);
        }

        /// <summary>
        /// 传输知识 (对应SkillDef)
        /// </summary>
        /// <param name="src">来源</param>
        /// <param name="dst">目标</param>
        /// <param name="percent">占来源的百分比</param>
        /// <param name="skillDef">技能类型</param>
        private void doTransferKnowledge(Pawn src, Pawn dst, float percent, SkillDef skillDef)
        {
            // 获取源的技能
            SkillRecord srcSkill = src?.skills?.GetSkill(skillDef);
            if (srcSkill == null)
            {
                Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge_Fail".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            // 获取目标的技能
            SkillRecord dstSkill = dst?.skills?.GetSkill(skillDef);
            if (dstSkill == null)
            {
                Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge_Fail".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            // 如果目标技能已经大于等于源技能，不起作用
            if (dstSkill.Level >= srcSkill.Level)
            {
                Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge_NoEffect".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            // 计算源和目标的总经验
            float srcTotalXp = srcSkill.XpTotalEarned + srcSkill.xpSinceLastLevel;
            float dstTotalXp = dstSkill.XpTotalEarned + dstSkill.xpSinceLastLevel;

            // 计算可转移的经验（不能超过两者经验差）
            float maxTransferXp = srcTotalXp - dstTotalXp;
            float transferXp = Mathf.Min(srcTotalXp * percent, maxTransferXp);

            Log.Message($"maxTransferXp: {maxTransferXp}");
            Log.Message($"transferXp: {transferXp}");

            dstSkill.Learn(transferXp, true, true);
        }

        // 抹除记忆
        private void ShapingCmdRemoveMemory()
        {


            // 获取并删除所有非基因的特质
            List<Trait> traits = parent.pawn?.story?.traits?.allTraits;
            if (traits != null && traits.Count > 0)
            {
                for (int i = traits.Count - 1; i >= 0; i--)
                {
                    if (traits[i].sourceGene != null)
                    {
                        // 基因的特质
                        continue;
                    }
                    parent.pawn.story.traits.RemoveTrait(traits[i]);

                }
            }

            // 获取所有的记忆和想法
            List<Thought_Memory> thoughts = parent.pawn?.needs?.mood?.thoughts?.memories?.Memories;
            if (thoughts != null && thoughts.Count > 0)
            {
                for (int i = thoughts.Count - 1; i >= 0; i--)
                {
                    parent.pawn.needs.mood.thoughts.memories.RemoveMemory(thoughts[i]);
                }
            }

            // 添加记忆
            addMasterMemory();

        }

    }
}
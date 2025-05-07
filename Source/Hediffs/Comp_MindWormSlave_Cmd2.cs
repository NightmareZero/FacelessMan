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
            // 传输知识 如上
            // 记忆抽取(从目标的特质中选一条，加入自己的特质，不能超过5条)
            // 记忆传输(从自己的特质中选一条，不能超过5条)
            menu.Add(new FloatMenuOption("nzflm.slave_transfer_knowledge".Translate(), delegate
            {
                SubMenu_ShapingCmd_TransferKnowledge();
            }, MenuOptionPriority.Default, null, null, 0f, null, null));
        }

        // 传输知识
        private void SubMenu_ShapingCmd_TransferKnowledge()
        {
            // 创建二级菜单
            List<FloatMenuOption> cmdMenu = new List<FloatMenuOption>
            {
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Fight".Translate(), delegate
                {
                    // TODO
                    Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Wisdom".Translate(), delegate
                {
                    // TODO
                    Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Work".Translate(), delegate
                {
                    // TODO
                    Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge".Translate(), MessageTypeDefOf.PositiveEvent);
                }),
                new FloatMenuOption("NzFLM.MindWormSlave_TransferKnowledge_Communicate".Translate(), delegate
                {
                    // TODO
                    Messages.Message("NzFaceLessManMod.MindWormSlave_TransferKnowledge".Translate(), MessageTypeDefOf.PositiveEvent);
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
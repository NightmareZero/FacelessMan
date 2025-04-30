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
        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (masterHediff == null)
            {
                yield break;
            }
            if (!masterHediff.CanMindAnything)
            {
                yield break;
            }

            yield return new Command_Action
            {
                defaultLabel = "NzFaceLessManMod.MindWormSlave_Gizmo".Translate(),
                defaultDesc = "NzFaceLessManMod.MindWormSlave_GizmoDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/MindWormSlave"),
                action = delegate
                {
                    var cmdMenu = new List<FloatMenuOption>();
                    if (masterHediff.CanMindCtrl) AddCtrlCmd(cmdMenu);
                    if (masterHediff.CanMindShaping) AddShapingCmd(cmdMenu);
                    if (masterHediff.CanMindCoverage) AddCoverageCmd(cmdMenu);
                    if (cmdMenu.Count == 0)
                    { return; }
                    FloatMenu menu = new FloatMenu(cmdMenu)
                    {
                        vanishIfMouseDistant = false,
                        // 弹出时暂停游戏
                        forcePause = true,
                        onCloseCallback = () =>
                        {
                            // TODO
#if DEBUG
                            Log.Message("flm: no select option");
#endif

                        }
                    };
                    Find.WindowStack.Add(menu);

                },
                hotKey = KeyBindingDefOf.Misc1,
            };



        }

        // 控制Gizmo
        private void AddCtrlCmd(List<FloatMenuOption> menu)
        {
            // 精神冲击
            menu.Add(new FloatMenuOption("nzflm.slaves_psychic_shock".Translate(),
             action: delegate
            {
                parent?.pawn?.AddHediffExt(HediffDefOf.PsychicShock, master, ticksToDisappear: 15000);
                Messages.Message("nzflm.slaves_psychic_shock_msg".Translate(parent.pawn.LabelCap),
                    MessageTypeDefOf.NeutralEvent);
            },
            itemIcon: null,
            iconColor: Color.white
            ));
            // 精神抚慰
            menu.Add(new FloatMenuOption("nzflm.slaves_psychic_soothe".Translate(),
             action: delegate
            {
                // 移除精神冲击
                if (parent?.pawn?.health.hediffSet?.TryGetHediff(HediffDefOf.PsychicShock, out var hediff) == true)
                {
                    parent?.pawn?.health?.RemoveHediff(hediff);
                }
                // 恢复精神状态
                if (parent?.pawn?.InMentalState == true)
                {
                    parent?.pawn?.MentalState?.RecoverFromState();
                }
                Messages.Message("nzflm.slaves_psychic_soothe_msg".Translate(parent.pawn.LabelCap),
                    MessageTypeDefOf.NeutralEvent);
            },
            itemIcon: null,
            iconColor: Color.white
            ));
        }

        private void AddShapingCmd(List<FloatMenuOption> menu)
        {
        }

        private void AddCoverageCmd(List<FloatMenuOption> menu)
        {
            if (parent?.pawn?.health?.hediffSet?.HasHediff(HediffDefsOf.NzFlm_He_MindWormCover) == true)
            {
                // 解除心智覆盖
                menu.Add(new FloatMenuOption("nzflm.slaves_psychic_coverage_remove".Translate(),
                 action: delegate
                {
                    // 移除心智覆盖Hediff
                    if (parent?.pawn?.health?.hediffSet?.TryGetHediff(HediffDefsOf.NzFlm_He_MindWormCover, out var hediff) == true)
                    {
                        parent?.pawn?.health?.RemoveHediff(hediff);
                    }
                    // 添加想法
                    Thought_MemorySocial thought = (Thought_MemorySocial)ThoughtMaker.MakeThought(DefsOf.NzFlm_Tk_ObsessedWithMaster);
                    thought.otherPawn = master;
                    parent.pawn?.needs?.mood?.thoughts?.memories?.TryGainMemory(thought, master);
                    // 输出消息
                    Messages.Message("nzflm.slaves_psychic_coverage_remove_msg".Translate(parent.pawn.LabelCap),
                        MessageTypeDefOf.NeutralEvent);
                },
                itemIcon: null,
                iconColor: Color.white
                ));
            }
            else
            {
                // 心智覆盖
                menu.Add(new FloatMenuOption("nzflm.slaves_psychic_coverage".Translate(),
                 action: delegate
                {
                    parent?.pawn?.AddHediffExt(HediffDefsOf.NzFlm_He_MindWormCover, master);
                    Messages.Message("nzflm.slaves_psychic_coverage_msg".Translate(parent.pawn.LabelCap),
                        MessageTypeDefOf.NeutralEvent);
                },
                itemIcon: null,
                iconColor: Color.white
                ));
            }
        }
    }
}
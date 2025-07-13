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
                defaultLabel = "NzFLM.MindWormSlave_Gizmo".Translate(),
                defaultDesc = "NzFLM.MindWormSlave_GizmoDesc".Translate(),
                // TODO 修改
                icon = ContentFinder<Texture2D>.Get("UI/Icons/Abilities/Psionic/MindShaping"),
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
                parent.pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).TryRandomElement(out var part);
                parent.pawn?.AddHediffExt(HediffDefOf.PsychicShock, caster: master, part, ticksToDisappear: 2500);

                // 播放音效
                DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);

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

                // 播放音效
                DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);

                Messages.Message("nzflm.slaves_psychic_soothe_msg".Translate(parent.pawn.LabelCap),
                    MessageTypeDefOf.NeutralEvent);
            },
            itemIcon: null,
            iconColor: Color.white
            ));
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
                    addMasterMemory();

                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);
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
                    parent.pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).TryRandomElement(out var part);
                    parent?.pawn?.AddHediffExt(HediffDefsOf.NzFlm_He_MindWormCover, master, part);

                    // 播放音效
                    DefsOf.Psycast_Skip_Pulse.PlayOneShot(parent.pawn);

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
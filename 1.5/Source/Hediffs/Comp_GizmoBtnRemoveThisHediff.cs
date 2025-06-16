using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    // 为Hediff添加一个按钮，点击后移除这个Hediff
    public class HediffCompProperties_GizmoBtnRemoveThis : HediffCompProperties
    {
        public string label;
        public string description;
        public string iconPath;

        // 移除后添加的Hediff
        public HediffDef afterRemovedHediff = null;

        // 如果不为 0 ， 则覆盖 afterRemovedHediff 自带的 severity
        public float severity = 0f;

        // 如果不为 0 ， 则覆盖 hediffComp_Disappears.ticksToDisappear
        public int dissapearAfter = 0;

        // 必须可以行动
        public bool mustBeAbleToAct = false;

        public HediffCompProperties_GizmoBtnRemoveThis()
        {
            compClass = typeof(HediffComp_GizmoBtnRemoveThis);
        }
    }

    public class HediffComp_GizmoBtnRemoveThis : HediffComp
    {
        public HediffCompProperties_GizmoBtnRemoveThis Props => (HediffCompProperties_GizmoBtnRemoveThis)props;

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Props.mustBeAbleToAct && !Pawn.Awake())
            {
                yield break;
            }

            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = Props.label == null ? "Remove ".Translate(parent.LabelCap) : Props.label;
            command_Action.defaultDesc = Props.description == null ? "Remove ".Translate(parent.LabelCap) : Props.description;
            command_Action.icon = ContentFinder<Texture2D>.Get(Props.iconPath, true);
            command_Action.action = delegate
            {
                Pawn.health.RemoveHediff(parent);
                if (Props.afterRemovedHediff != null)
                {

                    Hediff hediff2 = HediffMaker.MakeHediff(Props.afterRemovedHediff, Pawn);
                    if (Props.severity != 0)
                    {
                        hediff2.Severity = Props.severity;
                    }
                    if (Props.dissapearAfter != 0)
                    {
                        HediffComp_Disappears hediffComp_Disappears = hediff2.TryGetComp<HediffComp_Disappears>();
                        if (hediffComp_Disappears != null)
                        {
                            hediffComp_Disappears.ticksToDisappear = Props.dissapearAfter;
                        }
                    }
                    Pawn.health.AddHediff(hediff2);
                }
            };
            yield return command_Action;
        }

    }

}
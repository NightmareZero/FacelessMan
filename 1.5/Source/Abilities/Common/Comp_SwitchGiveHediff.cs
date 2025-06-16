using Verse;
using RimWorld;
using System;

namespace NzFaceLessManMod
{
    public class CompProperties_SwitchGiveHediff : CompProperties_AbilityGiveHediff
    {
        // 运作类型
        public string type = "add";

        // 使用Hediff自身的持续时间
        public bool useHediffDuration = false;

        public bool IsAdd => type == "add";
        public bool IsRemove => type == "remove";
        public bool IsSwitch => type == "switch";

        public CompProperties_SwitchGiveHediff()
        {
            compClass = typeof(CompAbilityGiveHediff_Switch);
        }
    }


    public class CompAbilityGiveHediff_Switch : CompAbilityEffect_GiveHediff
    {
        public new CompProperties_SwitchGiveHediff Props => (CompProperties_SwitchGiveHediff)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (!Props.ignoreSelf || target.Pawn != parent.pawn)
            {
                if (!Props.onlyApplyToSelf && Props.applyToTarget)
                {
                    ApplyInner(target.Pawn, parent.pawn);
                }

                if (Props.applyToSelf || Props.onlyApplyToSelf)
                {
                    ApplyInner(parent.pawn, target.Pawn);
                }
            }
        }

        protected new void ApplyInner(Pawn target, Pawn caster)
        {
            if (target == null)
            {
                return;
            }

            if (TryResist(target))
            {
                MoteMaker.ThrowText(target.DrawPos, target.Map, "Resisted".Translate());
                return;
            }

            // 获取存在
            Hediff existHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (Props.IsRemove || (existHediffOfDef != null && Props.IsSwitch))
            {
                target.health.RemoveHediff(existHediffOfDef);
                return;
            }


            if (Props.replaceExisting)
            {
                if (existHediffOfDef != null)
                {
                    target.health.RemoveHediff(existHediffOfDef);
                }
            }

            Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, target, Props.onlyBrain ? target.health.hediffSet.GetBrain() : null);
            HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
            if (hediffComp_Disappears != null && !Props.useHediffDuration)
            {
                hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(target).SecondsToTicks();
            }

            if (Props.severity >= 0f)
            {
                hediff.Severity = Props.severity;
            }

            HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
            if (hediffComp_Link != null)
            {
                hediffComp_Link.other = caster;
                hediffComp_Link.drawConnection = target == parent.pawn;
            }

            target.health.AddHediff(hediff);
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (parent.pawn.Faction == Faction.OfPlayer)
            {
                return false;
            }

            return target.Pawn != null;
        }

    }
}
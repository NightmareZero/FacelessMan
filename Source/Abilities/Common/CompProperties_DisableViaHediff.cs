using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NzFaceLessManMod
{
    public class CompProperties_DisableViaHediff : CompProperties_AbilityEffect
    {
        // 目标Hediff
        public HediffDef hediffDef = null;

        // 目标Hediff移除的时候，严重性将为0 (可以靠缓存提高性能)
        public bool zeroSeverityOnRemove = false;

        public CompProperties_DisableViaHediff()
        {
            compClass = typeof(CompAbility_DisableViaHediff);
        }

        public override IEnumerable<string> ConfigErrors(AbilityDef parentDef)
        {
            return (hediffDef == null ? new[] { "hediff is null" } : Array.Empty<string>())
                .Concat(base.ConfigErrors(parentDef));
        }
    }

    public class CompAbility_DisableViaHediff : CompAbilityEffect
    {
        private new CompProperties_DisableViaHediff Props => (CompProperties_DisableViaHediff)props;

        private Pawn Caster => parent.pawn;

        private Hediff _hediff = null;
        private Hediff TargetHediff
        {
            get
            {
                if (Props.zeroSeverityOnRemove)
                {
                    if (_hediff != null && _hediff.Severity <= 0f)
                    {
                        _hediff = null;
#if DEBUG
                        Log.Message($"[NzFaceLessManMod] {Caster} has {Props.hediffDef.label.Translate()} with severity <= 0f, set to null.");
#endif
                    }
                    if (_hediff == null)
                    {
                        _hediff = Caster.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
                        Log.Message($"[NzFaceLessManMod] {Caster} has no {Props.hediffDef.label.Translate()}, trying to get it. found: {_hediff != null}");
                    }
                    return _hediff;
                }

                return Caster.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            }
        }

        public override bool GizmoDisabled(out string reason)
        {
            if (Props.hediffDef != null && TargetHediff != null)
            {
                reason = "nzflm.has_target_status".Translate(Props.hediffDef.label.Translate());
                return true;
            }

            return base.GizmoDisabled(out reason);
        }

    }
}
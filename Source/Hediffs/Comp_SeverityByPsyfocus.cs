using Verse;
using RimWorld;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_SeverityByPsyfocus : HediffCompProperties
    {
        public HediffCompProperties_SeverityByPsyfocus()
        {
            compClass = typeof(HediffComp_SeverityByPsyfocus);
        }
    }

    public class HediffComp_SeverityByPsyfocus : HediffComp
    {
        public HediffCompProperties_SeverityByPsyfocus Props => (HediffCompProperties_SeverityByPsyfocus)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            // 每隔 300 ticks 更新一次
            if (parent.pawn.IsHashIntervalTick(300))
            {
                return;
            }

            // 获取当前 Pawn 的 PsychicEntropyTracker
            var psychicEntropy = Pawn.psychicEntropy;
            if (psychicEntropy != null)
            {
                // 根据公式计算 Severity
                parent.Severity = 0.01f + psychicEntropy.CurrentPsyfocus;
            }
        }
    }
}
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProp_AwakeMentalBreak : GeneExtCompProp
    {
        public int ticks = 300; // 5 seconds
        public CompProp_AwakeMentalBreak()
        {
            compClass = typeof(GeneExtComp_AwakeMentalBreak);
        }
    }

    public class GeneExtComp_AwakeMentalBreak : GeneExtComp
    {
        public CompProp_AwakeMentalBreak Props => (CompProp_AwakeMentalBreak)props;

        public override void CompPostTick()
        {
            if (parent.pawn.InMentalState && parent.pawn.IsHashIntervalTick(Props.ticks))
            {
                // 解除精神崩溃状态
                parent.pawn.MentalState.RecoverFromState();
                Messages.Message("nzflm.mental_break_recovered_by".Translate(parent.pawn.LabelShort), MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
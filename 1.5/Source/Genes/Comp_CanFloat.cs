using System;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProperties_CanFloat : GeneExtCompProp
    {
        public CompProperties_CanFloat()
        {
            compClass = typeof(GeneExtComp_CanFloat);
        }
    }

    public class GeneExtComp_CanFloat : GeneExtComp
    {
        public CompProperties_CanFloat Props => (CompProperties_CanFloat)props;

        public override void CompPostPostAdd()
        {
            base.CompPostPostAdd();

            // 设置可以漂浮
            PawnControl.SetCanMoveNoCost(Pawn);
        }
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            // 重置漂浮状态
            PawnControl.RstCanMoveNoCost(Pawn);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // 设置可以漂浮
                PawnControl.SetCanMoveNoCost(Pawn);
            }
        }
    }
}
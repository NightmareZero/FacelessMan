using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    [StaticConstructorOnStartup]
    public static class PawnControl
    {
        public static void Clear()
        {
            moveNoCostPawns.Clear();
        }

        // 可以漂浮
        private static HashSet<Pawn> moveNoCostPawns = new HashSet<Pawn>();

        public static bool CanMoveNoCost(Pawn pawn)
        {
            return moveNoCostPawns.Contains(pawn);
        }

        public static void SetCanMoveNoCost(Pawn pawn)
        {
            moveNoCostPawns.Add(pawn);
        }

        public static void RstCanMoveNoCost(Pawn pawn)
        {
            moveNoCostPawns.Remove(pawn);
        }

        public static bool CanMoveFloat(Pawn pawn)
        {
            if (pawn == null || pawn.DeadOrDowned || pawn.InMentalState)
            {
                return false;
            }

            if (moveNoCostPawns.Contains(pawn))
            {
                return true;
            }
            return false;
        }
    }
}
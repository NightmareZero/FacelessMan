using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    [StaticConstructorOnStartup]
    public static class AnimalControl
    {
        private static HashSet<Pawn> canDraft = new HashSet<Pawn>();

        private static HashSet<Pawn> noEscape = new HashSet<Pawn>();

        private static HashSet<Pawn> canUseAbility = new HashSet<Pawn>();

        public static void Clear()
        {
            canDraft.Clear();
            noEscape.Clear();
            canUseAbility.Clear();
        }

        public static bool CanDraft(Pawn pawn)
        {
            return canDraft.Contains(pawn);
        }

        public static void SetCanDraft(Pawn pawn)
        {
            canDraft.Add(pawn);
        }

        public static void ResetCanDraft(Pawn pawn)
        {
            canDraft.Remove(pawn);
        }

        public static bool CanDraftAndCtrl(Pawn pawn)
        {
            return CanDraft(pawn) && pawn.Faction?.IsPlayer == true && pawn.MentalState is null;
        }
        public static bool CanUseAbility(Pawn pawn)
        {
            return canUseAbility.Contains(pawn) && pawn.Faction?.IsPlayer == true && pawn.MentalState is null;
        }

        public static void SetCanUseAbility(Pawn pawn)
        {
            canUseAbility.Add(pawn);
        }

        public static void ResetCanUseAbility(Pawn pawn)
        {
            canUseAbility.Remove(pawn);
        }

        public static bool NoEscape(Pawn pawn)
        {
            return noEscape.Contains(pawn);
        }

        public static void SetNoEscape(Pawn pawn)
        {
            noEscape.Add(pawn);
        }

        public static void ResetNoEscape(Pawn pawn)
        {
            noEscape.Remove(pawn);
        }
    }
}
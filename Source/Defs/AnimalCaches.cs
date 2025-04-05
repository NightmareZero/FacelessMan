using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    [StaticConstructorOnStartup]
    public static class AnimalCaches
    {
        public static HashSet<Pawn> draftable_animals = new HashSet<Pawn>();

        public static HashSet<Pawn> nofleeing_animals = new HashSet<Pawn>();

        // A list of animals that can use abilities
        public static HashSet<Pawn> abilityUsing_animals = new HashSet<Pawn>();

        public static void Clear()
        { 
            draftable_animals.Clear();
            nofleeing_animals.Clear();
            abilityUsing_animals.Clear();
        }

        public static bool IsDraftableAnimal(this Pawn pawn)
        {
            return draftable_animals.Contains(pawn);
        }
        public static bool IsAbilityUserAnimal(this Pawn pawn)
        {
            return abilityUsing_animals.Contains(pawn) && pawn.Faction?.IsPlayer == true && pawn.MentalState is null;
        }
        public static bool IsDraftableControllableAnimal(this Pawn pawn)
        {
            return pawn.IsDraftableAnimal() && pawn.Faction != null && pawn.Faction.IsPlayer && pawn.MentalState is null;
        }

        public static void AddDraftableAnimalToList(Pawn pawn)
        {

            if (!draftable_animals.Contains(pawn))
            {
                draftable_animals.Add(pawn);
            }
        }

        public static void RemoveDraftableAnimalFromList(Pawn pawn)
        {
            if (draftable_animals.Contains(pawn))
            {
                draftable_animals.Remove(pawn);
            }

        }

        public static void AddAbilityUsingAnimalToList(Pawn pawn)
        {

            if (!abilityUsing_animals.Contains(pawn))
            {
                abilityUsing_animals.Add(pawn);
            }
        }

        public static void RemoveAbilityUsingFromList(Pawn pawn)
        {
            if (abilityUsing_animals.Contains(pawn))
            {
                abilityUsing_animals.Remove(pawn);
            }

        }

        public static void AddNotFleeingAnimalToList(Pawn pawn)
        {

            if (!nofleeing_animals.Contains(pawn))
            {
                nofleeing_animals.Add(pawn);
            }
        }

        public static void RemoveNotFleeingAnimalFromList(Pawn pawn)
        {
            if (nofleeing_animals.Contains(pawn))
            {
                nofleeing_animals.Remove(pawn);
            }

        }
    }
}
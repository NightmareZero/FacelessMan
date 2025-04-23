using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    /// <summary>
    /// Definition for an evolvable gene that carries prerequisite/exclusion logic
    /// and an evolution‑point cost used by the Faceless‑Man progression system.
    /// </summary>
    public class EvolutionGeneDef : GeneDef, IComparable<EvolutionGeneDef>, IExposable
    {
        /// <summary>Evolution point cost required to unlock this gene.</summary>
        public int EvolutionCost = 0;

        /// <summary>If true, the player cannot manually select this gene during xenotype creation.</summary>
        public bool CannotBeChosen = false;

        /// <remarks>
        ///   This hides <c>GeneDef.canGenerateInGeneSet</c> with a more conservative default.
        /// </remarks>
        public new bool canGenerateInGeneSet = false;

        /// <summary>Genes that must already exist in a genome before this one can be added.</summary>
        public List<EvolutionGeneDef> RequiredGenes = new();

        /// <summary>Genes that make this one unavailable if they are present.</summary>
        public List<EvolutionGeneDef> ExcludedGenes = new();

        /// <summary>Vanilla prerequisite (single gene).</summary>
        public GeneDef Prerequisite => prerequisite;

        /// <summary>Tags used by RimWorld to auto‑exclude conflicting genes.</summary>
        public IReadOnlyList<string> ExclusionTags => exclusionTags;

        public EvolutionGeneDef()
        {
            geneClass = typeof(GeneExt);
        }

        /// <inheritdoc/>
        public int CompareTo(EvolutionGeneDef other)
        {
            if (other == null) return 1;
            int order = displayOrderInCategory.CompareTo(other.displayOrderInCategory);
            return order == 0 ? defName.CompareTo(other.defName) : order;
        }

        /// <summary>Returns true if <paramref name="geneSet"/> satisfies all requirements for this gene.</summary>
        public bool RequirementsMet(HashSet<EvolutionGeneDef> geneSet)
        {
            if (geneSet == null) return false;

            foreach (var req in RequiredGenes)
                if (!geneSet.Contains(req)) return false;

            foreach (var excl in ExcludedGenes)
                if (geneSet.Contains(excl)) return false;

            return true;
        }

        /// <inheritdoc/>
        public void ExposeData()
        {
            Scribe_Values.Look(ref EvolutionCost, "evolutionCost", 0);
            Scribe_Values.Look(ref CannotBeChosen, "cannotBeChosen", false);
            Scribe_Values.Look(ref canGenerateInGeneSet, "canGenerateInGeneSet", false);
            Scribe_Collections.Look(ref RequiredGenes, "requiredGenes", LookMode.Def);
            Scribe_Collections.Look(ref ExcludedGenes, "excludedGenes", LookMode.Def);
        }

        /// <summary>Validate XML‑defined data; shows red errors in the mod list if invalid.</summary>
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var err in base.ConfigErrors())
                yield return err;

            if (EvolutionCost < 0)
                yield return $"{defName}: EvolutionCost cannot be negative.";

            if (RequiredGenes.Contains(null))
                yield return $"{defName}: RequiredGenes contains a null entry.";
        }
    }
}

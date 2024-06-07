using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;

namespace NzFaceLessManMod
{



    [HarmonyPatch(typeof(GeneDefGenerator))]
    [HarmonyPatch(nameof(GeneDefGenerator.ImpliedGeneDefs))]
    public static class FaceLessMan_GeneDefGenerator_ImpliedGeneDefs_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
        {
            List<GeneDef> genes = values.ToList();

            try
            {
                // 从xml中获取template
                XenoGeneTemplateDef template = null;

                GlobalValues.AvaliableXenotypeDef = DefDatabase<XenotypeDef>.AllDefs.Where(element => !Utils.XenotypeContainsGene(element, DefDatabase<GeneDef>.GetNamedSilentFail("VREA_Power"))
                && element.defName != "AG_RandomCustom").ToList();

                GlobalValues.AvaliableXenotypeDef.ForEach(xeno =>
                {
                    genes.Add(GetFromXenotype(template, xeno, genes.Count()));
                });
            }
            catch (Exception e)
            {
                Log.Error("Error in Postfix of GeneDefGenerator.ImpliedGeneDefs: " + e.Message);
            }


            return genes;
        }

        public static GeneDef GetFromXenotype(XenoGeneTemplateDef template, XenotypeDef def, int displayOrderBase)
        {


            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + def.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(def.label),
                iconPath = def.iconPath,
                description = template.description.Formatted(def.LabelCap),
                labelShortAdj = template.labelShortAdj.Formatted(def.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,

                modExtensions = new List<DefModExtension> {
                    new GeneXenoModExtension{
                        xenotypeDef = def

                    }
                },
                // modExtensions = new List<DefModExtension> {
                //     new VanillaGenesExpanded.GeneExtension {
                //         backgroundPathArchite = "UI/Icons/Genes/GeneBackground_MorphGene"
                //     },
                //     new MorphGeneDefExtension {
                //         xenotype = def
                //     }
                // },

                abilities = new List<AbilityDef> { template.ability },
                descriptionHyperlinks = new List<DefHyperlink> { new DefHyperlink { def = template.ability } }
            };


            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }

            return geneDef;

        }
    }

}
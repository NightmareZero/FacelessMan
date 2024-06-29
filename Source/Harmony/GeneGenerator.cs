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
                XenoGeneTemplateDef template = XmlDefs.xenoGeneTemplateDef;

                GlobalValues.AvaliableXenotypeDef = DefDatabase<XenotypeDef>.AllDefs.Where(element => !Utils.XenotypeContainsGene(element, DefDatabase<GeneDef>.GetNamedSilentFail("VREA_Power"))
                && element.defName != "AG_RandomCustom").ToList();

                GlobalValues.AvaliableXenotypeDef.ForEach(xeno =>
                {
                    genes.Add(GetFromXenotype(template, xeno, genes.Count()));
                    xeno.AllGenes.ForEach(gene =>
                    {
                        // 如果是超级载体则设置为包含所有基因
                        if (gene.defName == XmlDefs.Flm_SuperCarrier.defName)
                        {
                            GeneXenoModExtension modExt = new GeneXenoModExtension
                            {
                                containXeno = GlobalValues.AvaliableXenotypeDef.ToDictionary(x => x.defName, x => x)
                            };
                            // 如果没有modExtensions则创建一个
                            if (gene.modExtensions == null)
                            {
                                gene.modExtensions = new List<DefModExtension> { modExt };
                            }
                            else
                            {
                                gene.modExtensions.Add(modExt);
                            }
                        }
                    });
                });
            }
            catch (Exception e)
            {
                Log.Error("Error in Postfix of GeneDefGenerator.ImpliedGeneDefs: " + e.Message);
            }


            return genes;
        }

        public static GeneDef GetFromXenotype(XenoGeneTemplateDef template, XenotypeDef xeno, int displayOrderBase)
        {

            GeneXenoModExtension modExt = new GeneXenoModExtension
            {
                xenotypeDef = xeno
            };

            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + xeno.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(xeno.label),
                iconPath = xeno.iconPath,
                description = template.description.Formatted(xeno.LabelCap),
                labelShortAdj = template.labelShortAdj.Formatted(xeno.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,

                modExtensions = new List<DefModExtension> { modExt },

                descriptionHyperlinks = new List<DefHyperlink> { new DefHyperlink { def = template.ability } }
            };




            // Debug模式则输出日志
#if DEBUG
            Log.Message("flm: geneDef: " + geneDef.label);
#endif


            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }

            return geneDef;

        }
    }

}
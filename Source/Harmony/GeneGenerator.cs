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
        public static List<XenotypeDef> AvaliableXenotypeDef;
        [HarmonyPostfix]
        public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
        {
            #if DEBUG
            Log.Message("flm: GeneDefGenerator.ImpliedGeneDefs Postfix");
            #endif
            List<GeneDef> genes = values.ToList();

            try
            {
                // 从xml中获取template
                XenoGeneTemplateDef template = XmlDefs.xenoGeneTemplateDef;

                AvaliableXenotypeDef = DefDatabase<XenotypeDef>.AllDefs.Where(element => !Utils.XenotypeContainsGene(element, DefDatabase<GeneDef>.GetNamedSilentFail("VREA_Power"))
                && element.defName != "AG_RandomCustom").ToList();

                AvaliableXenotypeDef.ForEach(xeno =>
                {
#if DEBUG
                    Log.Message("flm: working on xenotype: " + xeno.label.Translate());
#endif
                    // 如果不是无面人异种，则生成一个对应基因包
                    if (xeno.defName != XmlDefs.Flm_FacelessMan.defName)
                    {
                        genes.Add(GetGenePackGeneFromXenotype(template, xeno, genes.Count()));
                    }
                    xeno.AllGenes.ForEach(gene =>
                    {
                        // 如果是超级载体则设置为包含所有基因
                        if (gene.defName == XmlDefs.Flm_GeneMaster.defName)
                        {

                            GeneXenoModExt modExt = new GeneXenoModExt
                            {
                                containXeno = AvaliableXenotypeDef.ToDictionary(x => x.defName, x => x)
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
                Log.Error("!Error in Postfix of GeneDefGenerator.ImpliedGeneDefs: " + e.Message);
                Log.Error("!StackTrace: " + e.StackTrace);
            }


            return genes;
        }

        public static GeneDef GetGenePackGeneFromXenotype(XenoGeneTemplateDef template, XenotypeDef xeno, int displayOrderBase)
        {

            GeneXenoModExt modExt = new GeneXenoModExt
            {
                xenotypeDef = xeno
            };

            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + xeno.defName,
                geneClass = template.geneClass,
                label = template.label.Translate(xeno.label),
                iconPath = xeno.iconPath,
                description = template.description.Translate(xeno.LabelCap),
                labelShortAdj = template.labelShortAdj.Translate(xeno.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                biostatArc = template.biostatArc,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
                modContentPack = template.modContentPack,
                modExtensions = new List<DefModExtension> { modExt },
            };

            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }

            return geneDef;

        }
    }

}
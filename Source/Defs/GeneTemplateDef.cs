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

namespace NzFaceLessManMod { 
    public class XenoGeneTemplateDef : Def
    {

        public Type geneClass = typeof(Gene);

        public int biostatCpx;

        public int biostatMet;

        public int biostatArc;

        public GeneCategoryDef displayCategory;

        public int displayOrderOffset;

        public float selectionWeight = 0f;


        [MustTranslate]
        public string labelShortAdj;

        [NoTranslate]
        public string iconPath;

        [NoTranslate]
        public string exclusionTagPrefix;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string item in base.ConfigErrors())
            {
                yield return item;
            }
            if (!typeof(Gene).IsAssignableFrom(geneClass))
            {
                yield return "geneClass is not Gene or subclass thereof.";
            }
        }
    }
}
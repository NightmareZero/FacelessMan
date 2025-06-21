using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace NzFaceLessManMod
{
    public static partial class Utils
    {
        public static void AddHediffSeverity(this Pawn pawn, HediffDef hediffDef, float severity, BodyPartRecord bodypart = null)
        {
            if (pawn == null || hediffDef == null || severity <= 0f)
            {
                return;
            }

            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            if (hediff != null)
            {
                hediff.Severity += severity;
            }
            else
            {
                hediff = HediffMaker.MakeHediff(hediffDef, pawn, bodypart);
                hediff.Severity += severity;
                pawn.health.AddHediff(hediff);
            }
        }
    }
}
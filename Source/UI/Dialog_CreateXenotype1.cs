using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using System;
using System.Linq;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public delegate void CustomGeneAction(CustomXenotype customXenotype);
    public class Dialog_CreateXenotype1 : Dialog_CreateXenotype
    {
        public override Vector2 InitialSize => new Vector2(Mathf.Min(UI.screenWidth, 1440), UI.screenHeight - 4);
        private CustomGeneAction callback;
        public Dialog_CreateXenotype1(int index, CustomGeneAction onAccept) : base(index, null)
        {
            callback = onAccept;
            forcePause = true;
        }

        protected override void Accept()
        {
            IEnumerable<string> warnings = GetWarnings();
            if (warnings.Any())
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("XenotypeBreaksLimits".Translate() + ":\n" + warnings.ToLineList("  - ", capitalizeItems: true) + "\n\n" + "SaveAnyway".Translate(), AcceptInner));
            }
            else
            {
                AcceptInner();
            }
        }

        private void AcceptInner()
        {
            CustomXenotype customXenotype = new CustomXenotype
            {
                name = xenotypeName?.Trim()
            };
            customXenotype.genes.AddRange(SelectedGenes);
            customXenotype.inheritable = false;
            customXenotype.iconDef = iconDef;
            string text = GenFile.SanitizedFileName(customXenotype.name);
            string absPath = GenFilePaths.AbsFilePathForXenotype(text);
            LongEventHandler.QueueLongEvent(delegate
            {
                GameDataSaveLoader.SaveXenotype(customXenotype, absPath);
            }, "SavingLongEvent", doAsynchronously: false, null);

            callback?.Invoke(customXenotype);
            Close();
        }

        private IEnumerable<string> GetWarnings()
        {
            if (ignoreRestrictions)
            {
                if (met > GeneTuning.BiostatRange.TrueMax)
                {
                    yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX")).CapitalizeFirst();
                }
                else if (met < GeneTuning.BiostatRange.TrueMin)
                {
                    yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN")).CapitalizeFirst();
                }
            }
        }


    }
}
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

        protected override void PostXenotypeOnGUI(float curX, float curY)
        {
            TaggedString taggedString2 = "IgnoreRestrictions".Translate();
            float width = Text.CalcSize(taggedString2).x + 4f + 24f;
            Rect rect = new Rect(curX, curY, width, Text.LineHeight);
            
            bool num = ignoreRestrictions;
            Widgets.CheckboxLabeled(rect, taggedString2, ref ignoreRestrictions);
            if (num != ignoreRestrictions)
            {
                if (ignoreRestrictions)
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("IgnoreRestrictionsConfirmation".Translate(), "Yes".Translate(), delegate
                    {
                    }, "No".Translate(), delegate
                    {
                        ignoreRestrictions = false;
                    }));
                }
                else
                {
                    SelectedGenes.RemoveAll((GeneDef x) => x.biostatArc > 0);
                    OnGenesChanged();
                }
            }

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, "IgnoreRestrictionsDesc".Translate());
            }

            postXenotypeHeight += rect.yMax - curY;
        }

        protected override bool CanAccept()
        {
            // 跳过基类的CanAccept以避免StartingPawnUtility的问题
            // 直接调用GeneCreationDialogBase的CanAccept
            if (xenotypeName.NullOrEmpty())
            {
                Messages.Message("MessageMustChooseLabel".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }

            if (!SelectedGenes.Any())
            {
                Messages.Message("MessageNoSelectedGenes".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }

            if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
            {
                Messages.Message("MessageTooManyCustomXenotypes", null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }

            if (!ignoreRestrictions && leftChosenGroups.Any())
            {
                Messages.Message("MessageConflictingGenesPresent".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }

            return true;
        }
    }
}
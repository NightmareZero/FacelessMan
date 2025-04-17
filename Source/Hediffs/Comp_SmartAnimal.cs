using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{
    public class HediffCompProperties_SmartAnimal : HediffCompProperties
    {
        public bool canDraft = true;
        public bool nonFleeing = true;
        public bool usingAbility = false;
        public bool train = true;

        public HediffCompProperties_SmartAnimal()
        {
            this.compClass = typeof(HediffComp_SmartAnimal);
        }
    }

    public class HediffComp_SmartAnimal : HediffComp
    {
        private HediffCompProperties_SmartAnimal Props => (HediffCompProperties_SmartAnimal)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (this.parent.pawn?.Faction?.IsPlayer == true && this.parent.pawn?.RaceProps?.Animal == true)
            {
                if (Props.canDraft)
                {
                    AnimalControl.SetCanDraft(this.parent.pawn);
                    if (parent.pawn.drafter is null)
                    {
                        parent.pawn.drafter = new Pawn_DraftController(parent.pawn);
                    }
                    if (parent.pawn.equipment is null)
                    {
                        parent.pawn.equipment = new Pawn_EquipmentTracker(parent.pawn);
                    }
                }
                if (Props.nonFleeing)
                {
                    AnimalControl.SetNoEscape(this.parent.pawn);
                }
                if (Props.usingAbility)
                {
                    AnimalControl.SetCanUseAbility(this.parent.pawn);
                }
                if (Props.train)
                {
                    doTrain();
                }
            }
        }

        private void doTrain()
        {
            // 确保召唤的动物属于玩家的Faction
            if (parent.pawn.Faction == Faction.OfPlayer)
            {
                // 完成顺从的所有训练
                foreach (var trainableDef in DefDatabase<TrainableDef>.AllDefsListForReading)
                {
                    if (parent.pawn.training.CanAssignToTrain(trainableDef))
                    {
                        parent.pawn.training.Train(trainableDef, null, true);
                        parent.pawn.training.SetWantedRecursive(trainableDef, true);
                    }
                }

            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (this.parent.pawn?.Faction?.IsPlayer == true && this.parent.pawn?.RaceProps?.Animal == true)
            {
                if (Props.canDraft)
                {
                    AnimalControl.ResetCanDraft(this.parent.pawn);
                }
                if (Props.nonFleeing)
                {
                    AnimalControl.ResetNoEscape(this.parent.pawn);
                }
                if (Props.usingAbility)
                {
                    AnimalControl.ResetCanUseAbility(this.parent.pawn);
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (this.parent.pawn?.Faction?.IsPlayer == true && this.parent.pawn?.RaceProps?.Animal == true)
                {
                    if (Props.canDraft)
                    {
                        AnimalControl.SetCanDraft(this.parent.pawn);
                        if (parent.pawn.drafter is null)
                        {
                            parent.pawn.drafter = new Pawn_DraftController(parent.pawn);
                        }
                        if (parent.pawn.equipment is null)
                        {
                            parent.pawn.equipment = new Pawn_EquipmentTracker(parent.pawn);
                        }
                    }
                    if (Props.nonFleeing)
                    {
                        AnimalControl.SetNoEscape(this.parent.pawn);
                    }
                    if (Props.usingAbility)
                    {
                        AnimalControl.SetCanUseAbility(this.parent.pawn);
                    }
                }
            }
        }
    }
}
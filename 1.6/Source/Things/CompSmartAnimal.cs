using Verse;
using RimWorld;

namespace NzFaceLessManMod
{

    public class CompProperties_SmartAnimal : CompProperties
    {
        // 是否可以征召
        public bool canDraft = true;
        // 征召后不会被袭击吓跑
        public bool nonFleeing = true;
        // 可以使用技能
        public bool usingAbility = false;
        public bool train = true;

        public CompProperties_SmartAnimal()
        {
            compClass = typeof(CompSmartAnimal);
        }
    }


    public class CompSmartAnimal : ThingComp
    {
        private CompProperties_SmartAnimal Props => (CompProperties_SmartAnimal)props;

        Pawn pawn => this.parent as Pawn;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (pawn == null || !pawn.RaceProps.Animal)
            {
                Log.Warning($"CompSmartAnimal can only be used on animals, but found {pawn?.def.defName ?? "null"}");
                return;
            }
            base.PostSpawnSetup(respawningAfterLoad);
            if (pawn?.Faction?.IsPlayer == true && pawn?.RaceProps?.Animal == true)
            {
                if (Props.canDraft)
                {
                    AnimalControl.SetCanDraft(pawn);
                    if (pawn.drafter is null)
                    {
                        pawn.drafter = new Pawn_DraftController(pawn);
                    }
                    if (pawn.equipment is null)
                    {
                        pawn.equipment = new Pawn_EquipmentTracker(pawn);
                    }
                }
                if (Props.nonFleeing)
                {
                    AnimalControl.SetNoEscape(pawn);
                }
                if (Props.usingAbility)
                {
                    AnimalControl.SetCanUseAbility(pawn);
                }
                if (Props.train)
                {
                    doTrain();
                }
            }
        }

        public override void CompTickLong()
        {
            base.CompTickLong();
            if (Props.train && pawn?.Faction?.IsPlayer == true
            && pawn?.RaceProps?.Animal == true
            && pawn?.IsHashIntervalTick(30000) == true)
            {
                doTrain();
            }
        }


        private void doTrain()
        {
            // 确保召唤的动物属于玩家的Faction
            if (pawn.Faction == Faction.OfPlayer)
            {
                // 完成顺从的所有训练
                foreach (var trainableDef in DefDatabase<TrainableDef>.AllDefsListForReading)
                {
                    if (pawn.training.CanAssignToTrain(trainableDef))
                    {
                        pawn.training.Train(trainableDef, null, true);
                        pawn.training.SetWantedRecursive(trainableDef, true);
                    }
                }

            }
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            PostRemove();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            PostRemove();
        }

        public void PostRemove()
        {
            if (pawn?.Faction?.IsPlayer == true && pawn?.RaceProps?.Animal == true)
            {
                if (Props.canDraft)
                {
                    AnimalControl.ResetCanDraft(pawn);
                }
                if (Props.nonFleeing)
                {
                    AnimalControl.ResetNoEscape(pawn);
                }
                if (Props.usingAbility)
                {
                    AnimalControl.ResetCanUseAbility(pawn);
                }
            }
        }

        public override void  PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (pawn?.Faction?.IsPlayer == true && pawn?.RaceProps?.Animal == true)
                {
                    if (Props.canDraft)
                    {
                        AnimalControl.SetCanDraft(pawn);
                        if (pawn.drafter is null)
                        {
                            pawn.drafter = new Pawn_DraftController(pawn);
                        }
                        if (pawn.equipment is null)
                        {
                            pawn.equipment = new Pawn_EquipmentTracker(pawn);
                        }
                    }
                    if (Props.nonFleeing)
                    {
                        AnimalControl.SetNoEscape(pawn);
                    }
                    if (Props.usingAbility)
                    {
                        AnimalControl.SetCanUseAbility(pawn);
                    }
                }
            }
        }
    }

}
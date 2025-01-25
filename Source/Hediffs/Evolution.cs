using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class Evolution : HediffWithComps
    {
        // 可用的点数
        public int evolutionLimit = 0;

        // 当前的点数
        public int evolution = 0;

        // 进化的基因
        public List<EvolutionGeneDef> evolutions = new List<EvolutionGeneDef>();

        public int lastEvolutionTick = 0;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (pawn.IsColonistPlayerControlled)
            {
                
                var manageEvolution = new Command_Action
                {
                    defaultLabel = "nzflm.evolution_button_label".Translate(),
                    defaultDesc = "nzflm.evolution_button_desc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Icons/Abilities/ViewGenes"),
                    action = () =>
                    {
                        // var options = new List<FloatMenuOption>();
                        // foreach (var allGeneline in GameComponent_Genelines.Instance.genelines.Where(x => x != geneline))
                        // {
                        //     options.Add(new FloatMenuOption(geneline != null ? "VRE_ChangeTo".Translate(allGeneline.name) : allGeneline.name, delegate
                        //     {
                        //         allGeneline.AddPawnWithMetapod(pawn);
                        //     }));
                        // }
                        // options.Add(new FloatMenuOption("VRE_ManageGenelines".Translate() + "...", delegate
                        // {
                        //     Find.WindowStack.Add(new Window_ManageGenelines());
                        // }));
                        // Find.WindowStack.Add(new FloatMenu(options));
                    }
                };
                if (pawn.health.hediffSet.GetFirstHediffOfDef(XmlDefs.Flm_GeneticInstability) != null)
                {
                    manageEvolution.Disable("VRE_MetapodSicknessCannotChangeGeneline".Translate());
                }
                yield return manageEvolution;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref evolutionLimit, "evolutionLimit");
            Scribe_Values.Look(ref evolution, "evolution");
            Scribe_Collections.Look(ref evolutions, "evolutions", LookMode.Def);
        }
    }
}
using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class Evolution : HediffWithComps, IGeneChangeListener
    {
        private bool geneDirty = false; // 是否需要变更
        private int geneDirtyTick = 0; // 需要变更tick
        // 可用的点数
        public int evolutionLimit = 0;
        // 当前的点数
        public int evolution = 0;
        // 上一次增加点数的时间
        public int lastPointAddTick = 0;

        private int pointAddInterval = 5 * 60000; // 5天增加一点

        // 进化的基因
        public List<EvolutionGeneDef> evolutions = new List<EvolutionGeneDef>();

        public override void Tick()
        {
            base.Tick();
            // 在dirty之后0.5秒执行
            if (geneDirty && Find.TickManager.TicksGame - geneDirtyTick > 30)
            {
                onGeneDirty();
                geneDirty = false;
                geneDirtyTick = 0;
            }

            // 如果已经可以新增点数
            if (Find.TickManager.TicksGame - lastPointAddTick > pointAddInterval)
            {
                this.evolutionLimit++; // 增加点数
                this.lastPointAddTick = Find.TickManager.TicksGame; // 记录时间
#if DEBUG
                Log.Message("evolutionLimit added to " + this.evolutionLimit);
#endif
            }
        }

        private void onGeneDirty()
        {
            GeneUtil.ApplyEvolutionToPawn(this, this.pawn); // 重新应用进化
        }

        public void MarkDirty()
        {
            this.geneDirty = true;
            this.geneDirtyTick = Find.TickManager.TicksGame;
        }

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
                        Find.WindowStack.Add(new Dialog_EditEvolution(pawn, delegate
                        {
                            // GameComponent_Genelines.Instance.AddGeneline(newGeneline);
                        }));
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
            Scribe_Values.Look(ref lastPointAddTick, "lastPointAddTick");
            Scribe_Values.Look(ref geneDirty, "geneDirty");
            Scribe_Values.Look(ref geneDirtyTick, "geneDirtyTick");
            Scribe_Collections.Look(ref evolutions, "evolutions", LookMode.Def);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
#if DEBUG
                Log.Warning("pointAddInterval set to 1 hour");
                pointAddInterval = 1 * 2500; // 每小时增加一点
#endif
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            if (this.evolutionLimit == 0)
            {
                this.evolutionLimit = 3;
            }
        }

        public void Notify_OnGeneChange(Gene gene, int action)
        {
            if (action != 0)
            {
                this.geneDirty = true; // 基因列表变更
            }
        }

    }
}
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

        private int pointAddInterval = 4 * 60000; // 4天增加一点

        public int evolutionMax = 30; // 最大点数

        private int lastCheckGeneticInstabilityTick = 0; // 上一次检查基因不稳定的时间
        private bool lastCheckGeneticInstabilityResult = true; // 上一次检查基因不稳定的结果

        public bool LastCheckGeneticInstabilityResult
        {
            get
            {
                if (Find.TickManager.TicksGame - lastCheckGeneticInstabilityTick > 600)
                {
                    lastCheckGeneticInstabilityResult = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefsOf.Flm_GeneticInstability) != null;
                    lastCheckGeneticInstabilityTick = Find.TickManager.TicksGame;
                }
                return lastCheckGeneticInstabilityResult;
            }
            set
            {
                lastCheckGeneticInstabilityResult = value;
                lastCheckGeneticInstabilityTick = Find.TickManager.TicksGame;
            }
        }

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

            if (this.evolutionLimit < this.evolutionMax)
            {
                int currentTick = Find.TickManager.TicksGame;
                // 确保lastPointAddTick已初始化，避免第一次就触发
                if (lastPointAddTick == 0)
                {
                    lastPointAddTick = currentTick;
                }
                else if (currentTick - lastPointAddTick > pointAddInterval)
                {
                    evolutionLimit++; // 增加点数
                    lastPointAddTick = currentTick; // 记录时间
#if DEBUG
                    Log.WarningOnce("evolutionLimit added to " + evolutionLimit, 62489671);
#endif
                }
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
                    Disabled = LastCheckGeneticInstabilityResult,
                    disabledReason = HediffDefsOf.Flm_GeneticInstability.label.Translate(),
                    action = () =>
                    {
                        Find.WindowStack.Add(new Dialog_EditEvolution(pawn, delegate
                        {
                            // GameComponent_Genelines.Instance.AddGeneline(newGeneline);
                        }));
                    }
                };
                if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefsOf.Flm_GeneticInstability) != null)
                {
                    manageEvolution.Disable(HediffDefsOf.Flm_GeneticInstability.label.Translate());
                }
                yield return manageEvolution;
            }
            // 玩家处于上帝模式
            if (DebugSettings.godMode)
            {
                var setPoint = new Command_Action
                {
                    defaultLabel = "set point 30",
                    defaultDesc = "",
                    icon = ContentFinder<Texture2D>.Get("UI/Icons/Abilities/ViewGenes"),
                    action = () =>
                    {
                        this.evolutionLimit = 30;
                        this.lastPointAddTick = Find.TickManager.TicksGame; // 记录时间
#if DEBUG
                        Log.Warning("evolutionLimit set to 30");
#endif
                    }
                };

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
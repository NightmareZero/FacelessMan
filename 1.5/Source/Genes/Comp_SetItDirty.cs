using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProp_SetItDirty : GeneExtCompProp
    {
        public int ticks = 300; // 5 seconds
        public bool enableOnTick = false;
        public bool enableOnAdd = true;
        public bool enableOnRemove = true;

        // 有效的Hediff列表
        public List<HediffDef> hediffs = new List<HediffDef>();

        public List<ThingDef> things = new List<ThingDef>();

        public List<GeneDef> genes = new List<GeneDef>();

        public CompProp_SetItDirty()
        {
            compClass = typeof(GeneExtComp_SetItDirty);
        }
    }

    public class GeneExtComp_SetItDirty : GeneExtComp
    {
        public CompProp_SetItDirty Props => (CompProp_SetItDirty)props;

        public override void CompPostTick()
        {
            if (Props.enableOnTick && parent.pawn.IsHashIntervalTick(Props.ticks))
            {
                SetDirtyForPawn(DirtyCondition.OnTick);
            }
        }

        public override void CompPostPostAdd()
        {
            if (Props.enableOnAdd)
            {
                SetDirtyForPawn(DirtyCondition.OnAdd);
            }
        }

        public override void CompPostPostRemoved()
        {
            if (Props.enableOnRemove)
            {
                SetDirtyForPawn(DirtyCondition.OnRemove);
            }
        }

        private void SetDirtyForPawn(DirtyCondition condition)
        {
            DirtyTrigger trigger = DirtyTrigger.Gene;
            // 更新Hediff
            if (Props.hediffs.Count > 0)
            {
                foreach (var hediff in parent.pawn.health.hediffSet.hediffs)
                {
                    if (Props.hediffs.Contains(hediff.def) && hediff is ISetDirty dirtyHediff)
                    {
                        dirtyHediff.SetDirty(trigger, condition, this.parent);
                    }
                }
            }

            // 更新物品
            if (Props.things.Count > 0)
            {
                foreach (var thing in parent.pawn.inventory.innerContainer)
                {
                    if (Props.things.Contains(thing.def) && thing is ISetDirty dirtyThing)
                    {
                        dirtyThing.SetDirty(trigger, condition, this.parent);
                    }
                }

                foreach (var equipment in parent.pawn.equipment.AllEquipmentListForReading)
                {
                    if (Props.things.Contains(equipment.def) && equipment is ISetDirty dirtyEquipment)
                    {
                        dirtyEquipment.SetDirty(trigger, condition, this.parent);
                    }
                }

                foreach (var apparel in parent.pawn.apparel.WornApparel)
                {
                    if (Props.things.Contains(apparel.def) && apparel is ISetDirty dirtyApparel)
                    {
                        dirtyApparel.SetDirty(trigger, condition, this.parent);
                    }
                }
            }

            // 更新基因
            if (Props.genes.Count > 0)
            {
                foreach (var gene in parent.pawn.genes.GenesListForReading)
                {
                    if (Props.genes.Contains(gene.def) && gene is ISetDirty dirtyGene)
                    {
                        dirtyGene.SetDirty(trigger, condition, this.parent);
                    }
                }
            }
        }
    }
}
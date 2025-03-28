using System;
using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{

    public class GeneExt : Gene
    {
        public GeneExtDef Def => def as GeneExtDef;
        public List<GeneExtComp> comps = new List<GeneExtComp>();

        public override string Label
        {
            get
            {
                string label = base.Label + "\n";
                if (comps != null)
                {
                    for (int i = 0; i < comps.Count; i++)
                    {
                        label += comps[i].LabelCap;
                    }
                }
                return label;
            }
        }

        public override string LabelCap => Label.CapitalizeFirst();

        public override void PostMake()
        {
            base.PostMake();
            InitializeComps();
            for (int num = comps.Count - 1; num >= 0; num--)
            {
                try
                {
                    comps[num].CompPostMake();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in HediffComp.CompPostMake(): " + ex);
                    comps.RemoveAt(num);
                }
            }
        }

        public override void PostAdd()
        {
            // 调用逻辑
            GeneDefModExt gDef = this.def.GetModExtension<GeneDefModExt>();
            if (gDef != null)
            {
                GeneDefModExt.ApplyGeneAdded(this); // 处理基因新增逻辑
            }
            // HediffWithComps
            // HediffCompProperties

            base.PostAdd(); // renderer

            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].CompPostPostAdd();
                }
            }
        }

        public override void PostRemove()
        {

            GeneDefModExt gDef = def.GetModExtension<GeneDefModExt>();
            if (gDef != null)
            {
                GeneDefModExt.ApplyRemoveGeneModExt(this); // 处理基因移除逻辑
            }

            base.PostRemove(); // renderer
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].CompPostPostRemoved();
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].CompPostTick();
                }
            }
        }


        public override IEnumerable<Gizmo> GetGizmos()
        {
            for (int i = 0; i < comps.Count; i++)
            {
                IEnumerable<Gizmo> enumerable = comps[i].CompGetGizmos();
                if (enumerable == null)
                {
                    continue;
                }

                foreach (Gizmo item in enumerable)
                {
                    yield return item;
                }
            }
        }

        public override void Notify_IngestedThing(Thing thing, int numTaken)
        {
            base.Notify_IngestedThing(thing, numTaken);
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].Notify_IngestedThing(thing, numTaken);
                }
            }
        }


        public override void Notify_NewColony()
        {
            base.Notify_NewColony();
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].Notify_NewColony();
                }
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].Notify_PawnDied(dinfo, culprit);
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].Reset();
                }
            }
        }


        private void InitializeComps()
        {
            if (Def.comps == null)
            {
                return;
            }

            comps = new List<GeneExtComp>();
            for (int i = 0; i < Def.comps.Count; i++)
            {
                GeneExtComp geneComp = null;
                try
                {
                    geneComp = (GeneExtComp)Activator.CreateInstance(Def.comps[i].compClass);
                    geneComp.props = Def.comps[i];
                    geneComp.parent = this;
                    comps.Add(geneComp);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not instantiate or initialize a HediffComp: " + ex);
                    comps.Remove(geneComp);
                }
            }


        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                InitializeComps();
            }

            if (comps != null)
            {
                for (int i = 0; i < comps.Count; i++)
                {
                    comps[i].CompExposeData();
                }
            }
        }

        public T GetComp<T>() where T : GeneExtComp
        {
            for (int i = 0; i < comps.Count; i++)
            {
                if (comps[i] is T)
                {
                    return comps[i] as T;
                }
            }

            return null;
        }
    }
}
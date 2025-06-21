using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class GeneCompProperties_DamageIgnore : GeneExtCompProp
    {
        public float chance = 0.5f; // 50% chance to ignore damage

        public float Change
        {
            get => Mathf.Clamp(chance, 0f, 1f);
        }

        public GeneCompProperties_DamageIgnore()
        {
            compClass = typeof(GeneComp_DamageIgnore);
        }
    }

    public class GeneComp_DamageIgnore : GeneExtComp
    {
        private GeneCompProperties_DamageIgnore Props => (GeneCompProperties_DamageIgnore)props;

        public override void CompPostPostAdd()
        {
            // 如果 Pawn 没有 ThingComp_DamageIgnore，则添加
            var tc = parent.pawn?.GetComp<ThingComp_DamageIgnore>();
            if (tc == null)
            {
                tc = new ThingComp_DamageIgnore().WithParent(parent.pawn);
                tc.chance = Props.Change;

                parent.pawn.AllComps.Add(tc);
            }
            else
            {
                // 如果已经存在，则更新 Chance
                tc.AddChance(Props.Change);
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();

            if (parent.pawn.GetComp<ThingComp_DamageIgnore>() is ThingComp_DamageIgnore comp)
            {
                comp.AddChance(-Props.Change);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // 如果 Pawn 没有 ThingComp_DamageIgnore，则添加
                var tc = parent.pawn?.GetComp<ThingComp_DamageIgnore>();
                if (tc == null)
                {
                    tc = new ThingComp_DamageIgnore().WithParent(parent.pawn);
                    tc.chance = Props.Change;

                    parent.pawn.AllComps.Add(tc);
                }
                else
                {
                    // 如果已经存在，则更新 Chance
                    tc.AddChance(Props.Change);
                }
            }
        }
    }

    
}
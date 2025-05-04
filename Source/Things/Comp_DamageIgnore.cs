using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{
    public class ThingComp_DamageIgnore : ThingComp
    {
        private Pawn parentPawn;

        public float chance = 0.5f; // 50% chance to ignore damage

        public void AddChance(float value)
        {
            chance = Mathf.Clamp(chance + value, 0f, 1f);
            if (chance <= 0.01f)
            {
                parentPawn?.AllComps.Remove(this);
            }
        }

        public ThingComp_DamageIgnore WithParent(Pawn pawn)
        {
            parentPawn = pawn;
            return this;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref parentPawn, "parentPawn");
            Scribe_Values.Look(ref chance, "chance", 0.5f); // 默认值为 0.5
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(ref dinfo, out absorbed);

            // 如果 Chance 大于随机数，则忽略伤害
            if (Rand.Value < chance)
            {
                absorbed = true; // 忽略伤害
                dinfo.SetAmount(0); // 设置伤害为 0
#if DEBUG
                Log.Message($"[NzFaceLessManMod] Damage ignored: {dinfo.Def} on {parentPawn.Name} with chance {chance}");
#endif
            }
        }
    }
}
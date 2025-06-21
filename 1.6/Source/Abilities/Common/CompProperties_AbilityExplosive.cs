using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompProperties_AbilityExplosive : CompProperties_AbilityEffect
    {
        public float explosiveRadius = 1.9f;

        public DamageDef explosiveDamageType;

        public int damageAmountBase = -1;

        public float armorPenetrationBase = -1f;

        public float chanceToStartFire;

        public EffecterDef explosionEffect;

        public SoundDef explosionSound;

        public CompProperties_AbilityExplosive()
        {
            compClass = typeof(CompAbilityExplosive);
        }

    }
}
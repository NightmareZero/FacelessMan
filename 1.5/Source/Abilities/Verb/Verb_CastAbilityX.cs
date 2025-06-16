using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class Verb_CastAbilityX : Verb_CastAbility
    {
        public override float EffectiveRange => FinalRange(this, CasterPawn);

        public float FinalRange(Verb ownerVerb, Pawn attacker)
        {
            if (ownerVerb.verbProps.rangeStat == null)
            {
                return ownerVerb.verbProps.range;
            }

            return ownerVerb.verbProps.range * attacker.GetStatValue(ownerVerb.verbProps.rangeStat, true, cacheStaleAfterTicks: 600);
        }
    }
}
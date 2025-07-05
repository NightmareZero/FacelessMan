using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class Verb_AbilityShootX : Verb_AbilityShoot
    {
        public float AdjustedRange(Verb ownerVerb, Pawn attacker)
        {
            if (verbProps.rangeStat == null)
            {
                return verbProps.range;
            }
            else if (verbProps.rangeStat != null && verbProps.range == 0)
            {
                return attacker.GetStatValue(verbProps.rangeStat, cacheStaleAfterTicks: 600);
            }


            return attacker.GetStatValue(verbProps.rangeStat, cacheStaleAfterTicks: 600) * verbProps.range;
        }

        public override float EffectiveRange => AdjustedRange(this, CasterPawn);

        int shootCount = 0;
        protected override bool TryCastShot()
        {
            shootCount++;
            if (shootCount >= BurstShotCount)
            {
                shootCount = 0;
                return Ability.Activate(currentTarget, currentDestination);
            }
            return base.TryCastShot();
        }
    }
}
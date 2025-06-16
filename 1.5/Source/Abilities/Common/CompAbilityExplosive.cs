using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityExplosive : CompAbilityEffect
    {

        public new CompProperties_AbilityExplosive Props => (CompProperties_AbilityExplosive)props;

        public Pawn Caster => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Cell.IsValid)
            {
                GenExplosion.DoExplosion(
                   center: target.Cell,
                   map: parent.pawn.Map,
                   radius: Props.explosiveRadius,
                   damType: Props.explosiveDamageType,
                   instigator: parent.pawn,
                   damAmount: Props.damageAmountBase,
                   armorPenetration: Props.armorPenetrationBase,
                   explosionSound: Props.explosionSound,
                   weapon: null,
                   projectile: null,
                   intendedTarget: null,
                   postExplosionSpawnThingDef: null,
                   postExplosionSpawnChance: 0f,
                   postExplosionSpawnThingCount: 1,
                   postExplosionGasType: null,
                   applyDamageToExplosionCellsNeighbors: false,
                   preExplosionSpawnThingDef: null,
                   preExplosionSpawnChance: 0f,
                   preExplosionSpawnThingCount: 1,
                   chanceToStartFire: Props.chanceToStartFire,
                   damageFalloff: false,
                   direction: null,
                   ignoredThings: null,
                   affectedAngle: null,
                   doVisualEffects: true,
                   propagationSpeed: 1f,
                   excludeRadius: 0f,
                   doSoundEffects: true,
                   postExplosionSpawnThingDefWater: null,
                   screenShakeFactor: 1f,
                   flammabilityChanceCurve: null,
                   overrideCells: null
               );

            }

        }
        
        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            if (Props.explosionEffect != null)
            {
                yield return new PreCastAction
                {
                    action = delegate (LocalTargetInfo a, LocalTargetInfo b)
                    {
                        parent.AddEffecterToMaintain(Props.explosionEffect.Spawn(parent.pawn.Position, a.Cell, parent.pawn.Map), Caster.Position, a.Cell, 17, Caster.MapHeld);
                    },
                    ticksAwayFromCast = 17
                };
            }
        }

    }
}
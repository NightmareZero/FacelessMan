using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityEffect_RangeExplosion : CompAbilityEffect
    {
        private new CompProperties_AbilityRangeExplosion Props => (CompProperties_AbilityRangeExplosion)props;

        private Pawn Caster => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            // 计算伤害和穿甲
            CalculateDamageAndArmorPenetration(out int damage, out float armorPenetration);

            var effectedPawn = new HashSet<Pawn>();

            for (int i = 0; i < Props.damageTimes; i++)
            {
                IntVec3 targetCell = target.Cell;
                if (Props.deviationRange > 0)
                {
                    targetCell = TargetAffectedCells(target).RandomElement();
                }
                var damageType = Props.GetOrderedDamageType();
                var explosionCells = GetRangeCells(targetCell, Props.explosionRange);

                // 如果需要添加hediff
                if (Props.GetHediffDefs().Count > 0)
                {
                    // 获得范围内的pawn
                    foreach (var cell in explosionCells)
                    {
                        var pawn = cell.GetFirstPawn(Caster.Map);
                        if (pawn != null)
                        {
#if DEBUG
                            Log.Message("RangeExplosion affected pawn: " + pawn);
#endif
                            effectedPawn.Add(pawn);
                        }

                    }
                }

                GenExplosion.DoExplosion(target.Cell, parent.pawn.MapHeld, Props.explosionRange, damageType, Caster,
                    postExplosionSpawnThingDef: Props.filthDef, damAmount: damage, armorPenetration: armorPenetration, explosionSound: Props.explosionSound, weapon: null,
                    projectile: null, intendedTarget: null, postExplosionSpawnChance: 1f, postExplosionSpawnThingCount: 1, postExplosionGasType: null,
                    applyDamageToExplosionCellsNeighbors: false, preExplosionSpawnThingDef: null, preExplosionSpawnChance: 0f, preExplosionSpawnThingCount: 1,
                    chanceToStartFire: 0f, damageFalloff: false, direction: null, ignoredThings: null, affectedAngle: null, doVisualEffects: true,
                    propagationSpeed: 0.6f, excludeRadius: 0f, doSoundEffects: true, postExplosionSpawnThingDefWater: null, screenShakeFactor: 1f,
                    flammabilityChanceCurve: parent.verb.verbProps.flammabilityAttachFireChanceCurve, overrideCells: explosionCells);
            }

            // 添加hediff
            if (Props.GetHediffDefs().Count > 0)
            {
                foreach (var pawn in effectedPawn)
                {
                    Props.GetHediffDefs().ForEach(hediffDef =>
                    {
                        pawn.health.AddHediff(hediffDef);
                    });
                }
            }

            base.Apply(target, dest);

        }

        /// <summary>
        /// 计算伤害和穿甲
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="armorPenetration"></param>
        private void CalculateDamageAndArmorPenetration(out int damage, out float armorPenetration)
        {
            damage = Props.damage;
            armorPenetration = Props.armorPenetration;
            if (Props.damageFactorStat != null)
            {
                damage *= (int)Caster.GetStatValue(Props.damageFactorStat);
            }
            if (Props.armorPenetrationFactorStat != null)
            {
                armorPenetration *= Caster.GetStatValue(Props.armorPenetrationFactorStat);
            }
        }

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            if (Props.effecterDef != null)
            {
                yield return new PreCastAction
                {
                    action = delegate (LocalTargetInfo a, LocalTargetInfo b)
                    {
                        parent.AddEffecterToMaintain(Props.effecterDef.Spawn(parent.pawn.Position, a.Cell, parent.pawn.Map), Caster.Position, a.Cell, 17, Caster.MapHeld);
                    },
                    ticksAwayFromCast = 17
                };
            }
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(TargetAffectedCells(target), Color.red);
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return false;
        }

        private List<IntVec3> TargetAffectedCells(LocalTargetInfo target)
        {
            IntVec3 intVec = target.Cell.ClampInsideMap(Caster.Map);
            if (Caster.Position == intVec)
            {
                return new List<IntVec3>();
            }

            float targetRange = Props.explosionRange; // 获取 targetRange 的值
            IntVec3 targetCell = target.Cell; // 获取目标位置

            return GetRangeCells(targetCell, targetRange);
        }

        private List<IntVec3> GetRangeCells(IntVec3 targetCell, float targetRange)
        {
            List<IntVec3> cells = new List<IntVec3>();
            foreach (IntVec3 intVec2 in GenRadial.RadialCellsAround(targetCell, targetRange, true))
            {
                if (CanUseCell(intVec2))
                {
                    cells.Add(intVec2);
                }
            }

            return cells;
        }

        private bool CanUseCell(IntVec3 c)
        {

            if (!c.InBounds(Caster.Map))
            {
                return false;
            }

            if (c == Caster.Position)
            {
                return false;
            }

            return true;
        }

    }
}
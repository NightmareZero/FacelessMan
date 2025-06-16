using RimWorld;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Security.Cryptography;
using static UnityEngine.GraphicsBuffer;

namespace NzFaceLessManMod
{
    public class Projectile_HediffOnly : Projectile
    {
        public override bool AnimalsFleeImpact => true;

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            blockedByShield = false; // 强制为false
            Map map = base.Map;
            IntVec3 position = base.Position;
            base.Impact(hitThing, false);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null && hitThing is Pawn victim && !victim.Dead)
            {
                bool instigatorGuilty = !(launcher is Pawn pawn) || !pawn.Drafted;
                DamageInfo dinfo = new DamageInfo(def.projectile.damageDef, DamageAmount, ArmorPenetration, ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
                // hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn2 = hitThing as Pawn;
                
                if (dinfo.Def.hediff != null && pawn2 != null)
                {
                    HediffDef hediffDef = dinfo.Def.hediff;
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn2, null);
                    hediff.Severity = hediffDef.initialSeverity;
                    pawn2.health.AddHediff(hediff, null, null, null);
                    if (dinfo.Def.additionalHediffs != null)
                    {
                        foreach (DamageDefAdditionalHediff hediffDef2 in dinfo.Def.additionalHediffs)
                        {
                            Hediff hediff2 = HediffMaker.MakeHediff(hediffDef2.hediff, pawn2, null);
                            hediff2.Severity = hediffDef2.hediff.initialSeverity;
                            pawn2.health.AddHediff(hediff2, null, null, null);
                        }
                    }
                }
                // 播放音效
                if (victim.SpawnedOrAnyParentSpawned)
                {
                    ImpactSoundUtility.PlayImpactSound(victim, dinfo.Def.impactSoundType, victim.MapHeld);
                }
                if (dinfo.Def.ExternalViolenceFor(this))
                {
                    if (dinfo.SpawnFilth)
                    {
                        GenLeaving.DropFilthDueToDamage(this, dinfo.Amount);
                    }
                }
                 if (dinfo.Def.damageEffecter != null)
                {
                    Effecter effecter = dinfo.Def.damageEffecter.Spawn();
                    effecter.Trigger(pawn2, pawn2);
                    effecter.Cleanup();
                }

                if (def.projectile.extraDamages != null)
                {
                    foreach (ExtraDamage extraDamage in def.projectile.extraDamages)
                    {
                        if (Rand.Chance(extraDamage.chance))
                        {
                            DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
                            // hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                            if (dinfo2.Def.hediff != null && pawn2 != null)
                            {
                                HediffDef hediffDef = dinfo2.Def.hediff;
                                Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn2, null);
                                hediff.Severity = hediffDef.initialSeverity;
                                pawn2.health.AddHediff(hediff, null, null, null);
                            }
                            if (dinfo2.Def.additionalHediffs != null)
                            {
                                foreach (DamageDefAdditionalHediff hediffDef2 in dinfo2.Def.additionalHediffs)
                                {
                                    Hediff hediff2 = HediffMaker.MakeHediff(hediffDef2.hediff, pawn2, null);
                                    hediff2.Severity = hediffDef2.hediff.initialSeverity;
                                    pawn2.health.AddHediff(hediff2, null, null, null);
                                }
                            }
                            // 播放音效
                            if (victim.SpawnedOrAnyParentSpawned)
                            {
                                ImpactSoundUtility.PlayImpactSound(victim, dinfo2.Def.impactSoundType, victim.MapHeld);
                            }
                            if (dinfo2.Def.ExternalViolenceFor(this))
                            {
                                if (dinfo2.SpawnFilth)
                                {
                                    GenLeaving.DropFilthDueToDamage(this, dinfo2.Amount);
                                }
                            }
                            if (dinfo2.Def.damageEffecter != null)
                            {
                                Effecter effecter = dinfo2.Def.damageEffecter.Spawn();
                                effecter.Trigger(pawn2, pawn2);
                                effecter.Cleanup();
                            }
                        }
                    }


                    if (Rand.Chance(def.projectile.bulletChanceToStartFire) && (pawn2 == null || Rand.Chance(FireUtility.ChanceToAttachFireFromEvent(pawn2))))
                    {
                        hitThing.TryAttachFire(def.projectile.bulletFireSizeRange.RandomInRange, launcher);
                    }

                    return;
                }

                if (!blockedByShield)
                {
                    SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map));
                    if (base.Position.GetTerrain(map).takeSplashes)
                    {
                        FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 4f);
                    }
                    else
                    {
                        FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
                    }
                }

                if (Rand.Chance(def.projectile.bulletChanceToStartFire))
                {
                    FireUtility.TryStartFireIn(base.Position, map, def.projectile.bulletFireSizeRange.RandomInRange, launcher);
                }
            }


        }
    }
}
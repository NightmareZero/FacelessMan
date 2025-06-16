using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;



namespace NzFaceLessManMod
{
    public class CompProperties_Hit : CompProperties_AbilityEffect
    {
        // 伤害类型
        public DamageDef damageDef;
        public DamageDef DamageDef
        {
            get
            {
                if (damageDef == null)
                {
                    damageDef = DamageDefOf.Blunt;
                }
                return damageDef;
            }
        }

        // 伤害数值
        public float damageAmountBase;
        // 穿甲
        public float armorPenetrationBase = 0;
        // 伤害放大
        public StatDef damageMultiplierStat;
        // 穿甲放大
        public StatDef armorPenetrationMultiplierStat;
        // 打击部位
        public List<BodyPartDef> hitParts = null;
        // 随机/顺序获取
        public bool hitPartRandom = false;

        public BodyPartRecord GetHitPart(Pawn target)
        {

            try
            {
                if (hitParts == null || hitParts.Count == 0)
                {
                    // 随机获取
                    return target.health?.hediffSet?.GetNotMissingParts().RandomElement();
                }
                if (hitPartRandom)
                {
                    // 选定部位随机获取
                    return target.health?.hediffSet?.GetNotMissingParts()
                        .Where(x => hitParts.Contains(x.def))
                        .RandomElement();
                }
                else
                {
                    // 选定部位顺序获取
                    foreach (var partDef in hitParts)
                    {
                        var part = target.health?.hediffSet?.GetNotMissingParts()
                            .FirstOrDefault(x => x.def == partDef);
                        if (part != null)
                        {
                            return part;
                        }
                    }
                    return null; // 如果没有找到匹配的部位，返回 null
                }
            }
            catch (Exception e)
            {
                Log.Error($"NzFaceLessManMod: GetHitPart error: {e}");
                return null;
            }

        }
        // 声音
        public SoundDef soundHitPawn;

        public SoundDef SoundHitPawn
        {
            get
            {
                if (soundHitPawn == null)
                {
                    soundHitPawn = SoundDefOf.Pawn_Melee_Punch_HitPawn;
                }
                return soundHitPawn;
            }
        }
        // 击晕时间
        public int stunTicks = 0;

        public CompProperties_Hit()
        {
            compClass = typeof(CompAbilityEffect_Hit);
        }


    }
}
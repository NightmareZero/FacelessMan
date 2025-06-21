using Verse;
using RimWorld;
using System;

namespace NzFaceLessManMod
{
    public class CompProperties_SummonCreature : CompProperties_AbilityEffect
    {
        // 召唤生物
        public PawnKindDef pawnKindDef;
        // 召唤数量
        public int countMin = 1;
        public int countMax = 1;
        // 召唤位置 true 目标点 false 自己身上
        public bool onTarget = true;
        // 召唤生物的Faction (null则为玩家Faction)
        public Faction faction = null;

        // 召唤生物附加的Hediff
        public HediffDef addHediff = null;

        public CompProperties_SummonCreature()
        {
            compClass = typeof(CompAbility_SummonCreature);
        }
    }

    public class CompAbility_SummonCreature : CompAbilityEffect
    {
        public new CompProperties_SummonCreature Props => (CompProperties_SummonCreature)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (Props.pawnKindDef == null) return;

            if (Props.countMin > 1 || Props.countMax > 1)
            {
                int count = Rand.RangeInclusive(Props.countMin, Props.countMax);
                for (int i = 0; i < count; i++)
                {
                    SummonCreature(target);
                }
            }
        }

        private void SummonCreature(LocalTargetInfo target)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(Props.pawnKindDef, Props.faction ?? parent.pawn.Faction);
            if (pawn != null)
            {
                IntVec3 cell = Props.onTarget ? target.Cell : parent.pawn.Position;
                GenSpawn.Spawn(pawn, cell, parent.pawn.Map);
                if (Props.addHediff != null)
                {
                    pawn.health.AddHediff(Props.addHediff);
                }
            }
        }
    }
}
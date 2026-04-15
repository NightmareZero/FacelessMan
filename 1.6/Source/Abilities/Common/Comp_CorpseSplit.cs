using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace NzFaceLessManMod
{
    public class Comp_CorpseSplit : CompAbilityEffect
    {
        public new CompProperties_CorpseSplit Props => (CompProperties_CorpseSplit)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Corpse corpse = target.Thing as Corpse;
            if (corpse == null)
            {
                return;
            }

            // 计算体型并确定生成的碎肉块数量
            float bodySize = corpse.InnerPawn.BodySize;
            int chunkCount = Mathf.RoundToInt(bodySize / 0.5f * 3);
            chunkCount = Mathf.Max(1, chunkCount); // 至少生成一个

            // 生成碎肉块
            for (int i = 0; i < chunkCount; i++)
            {
                // 创建碎肉块生物
                PawnKindDef fleshChunkKind = PawnKindDef.Named("FleshChunk");
                Pawn fleshChunk = PawnGenerator.GeneratePawn(new PawnGenerationRequest(fleshChunkKind, null, PawnGenerationContext.NonPlayer));
                
                // 立即杀死碎肉块，使其成为尸体状态
                fleshChunk.Kill(null);
                
                // 放置碎肉块尸体
                Corpse fleshChunkCorpse = fleshChunk.Corpse;
                if (fleshChunkCorpse != null)
                {
                    GenPlace.TryPlaceThing(fleshChunkCorpse, corpse.Position, corpse.Map, ThingPlaceMode.Near);
                }
            }

            // 销毁原尸体
            corpse.Destroy(DestroyMode.Vanish);
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Corpse corpse = target.Thing as Corpse;
            return corpse != null && base.CanApplyOn(target, dest);
        }
    }
}
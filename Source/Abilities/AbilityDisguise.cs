using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
	[StaticConstructorOnStartup]
	public class CompAbilityDisguise : CompAbilityEffect
	{

		public override IEnumerable<PreCastAction> GetPreCastActions()
		{
			return base.GetPreCastActions();
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);

			Pawn caster = parent.pawn;
			Pawn targetPawn = target.Pawn;
			if (targetPawn == null)
			{
				Log.Message("targetPawn is null");
				return;
			}


			// 处理伪装逻辑
			Disguise(caster, targetPawn);

		}

		public static void Disguise(Pawn caster, Pawn target)
		{
			// 伪装逻辑的实现
			// 这可能包括改变外观、技能、能力等
			// 具体实现取决于你的游戏逻辑和需求

            // 设置头发为目标的头发
            caster.story.hairDef = target.story.hairDef;
            caster.story.HairColor = target.story.HairColor;
            // 设置脸型为目标的脸型
            caster.story.headType = target.story.headType;
            // 设置体型
            caster.story.bodyType = target.story.bodyType;
            // 设置肤色
            caster.story.skinColorOverride = target.story.SkinColor;
            // 设置胡须
            caster.story.furDef = target.story.furDef;
            // 播放声音
            XmlDefs.FoamSpray_Resolve.PlayOneShot(new TargetInfo(target.Position, target.Map, false));

			// 重新渲染caster
			caster.Drawer.renderer.SetAllGraphicsDirty();
		}


	}
}
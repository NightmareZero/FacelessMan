using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityMorphX : CompAbilityEffect
    {

        private bool isXenoSelected = false;

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            return base.GetPreCastActions();
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn casterPawn = parent.pawn;

            List<FloatMenuOption> selectXenoMenu = new List<FloatMenuOption>();

            // 从施放者的基因中寻找可用的基因组
            Dictionary<string, XenotypeDef> xenoGenes = Utils.GetGeneXenotypes(casterPawn);

            Log.Message("xenoGenes.Count: " + xenoGenes.Count);
            if (xenoGenes.Count == 0)
            {
                // 在左上角弹出消息
                Messages.Message("nzflm.no_xenotype".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            if (xenoGenes.Count == 1)
            {
                // 如果只有一个基因, 则直接使用第一个
                MorphXenotype(casterPawn, xenoGenes.First().Value);
                this.isXenoSelected = true;
                base.Apply(target, dest);
                return;
            }

            // 绘制菜单
            foreach (var xeno_ in xenoGenes)
            {
                var xeno = xeno_;
                if (xeno.Value.defName == DefsOf.Flm_FacelessMan.defName)
                {
                    continue;
                }
#if DEBUG
                Log.Message("flm: draw menu xeno.Key: " + xeno.Key);
#endif
                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    MorphXenotype(casterPawn, xeno.Value);
                    this.isXenoSelected = true;
                    base.Apply(target, dest);
                });
                selectXenoMenu.Add(opt);
            }

            // 显示 FloatMenu
            FloatMenu menu = new FloatMenu(selectXenoMenu)
            {
                vanishIfMouseDistant = false,
                // 弹出时暂停游戏
                forcePause = true,


                onCloseCallback = () =>
                {
                    // 如果未选择任何选项, 则移除CD
                    if (isXenoSelected == false)
                    {
                        // 在左上角输出内容
                        Messages.Message("nzflm.no_xenotype_selected".Translate(), MessageTypeDefOf.RejectInput);
                        this.parent.ResetCooldown();
                    }
#if DEBUG
                    Log.Message("flm: no select option");
#endif

                }
            };

            Find.WindowStack.Add(menu);
        }

        /// <summary>
        /// 从施放者身上, 移除一个基因, 并将其移植到目标身上
        /// </summary>
        public static void MorphXenotype(Pawn caster, XenotypeDef xeno)
        {
            // 清除旧基因
            CleanOldXenotype(caster);
            // 添加新基因
            AddNewXenotypeGenes(caster, xeno);
            // 修复变身技能
            fixMorphAbility(caster);
            // 播放声音
            DefsOf.FoamSpray_Resolve.PlayOneShot(new TargetInfo(caster.Position, caster.Map, false));
        }

        /// <summary>
        /// 修复变身技能, 防止在有多个变身基因的情况下，移除一个基因后，变身技能消失
        /// </summary>
        /// <param name="caster"></param>
        private static void fixMorphAbility(Pawn caster)
        { 
            // 检查caster是否有变身技能
            Ability morphAbility = caster.abilities?.abilities?.Find(a => a.def == DefsOf.Flm_Morphing);
            if (morphAbility != null) // 如果有，则不需要修复
            { 
                return; 
            }

            // 检查基因中是否有变身技能
            if (caster?.genes == null)
            {
                return;
            }
            caster.genes?.Endogenes?.ForEach(gene =>
            {
                if (gene.def?.abilities != null)
                {
                    gene.def.abilities.ForEach(abilityDef =>
                    {
                        if (abilityDef == DefsOf.Flm_Morphing)
                        {
                            // 添加变身技能
                            Ability ability = new Ability(caster,DefsOf.Flm_Morphing);
                            caster.abilities.abilities.Add(ability);
                            return; // 找到一个就可以了
                        }
                    });
                }
            });

            caster.genes?.Xenogenes?.ForEach(gene =>
            {
                if (gene.def?.abilities != null)
                {
                    gene.def.abilities.ForEach(abilityDef =>
                    {
                        if (abilityDef == DefsOf.Flm_Morphing)
                        {
                            // 添加变身技能
                            Ability ability = new Ability(caster, DefsOf.Flm_Morphing);
                            caster.abilities.abilities.Add(ability);
                            return; // 找到一个就可以了
                        }
                    });
                }
            });

        }

        /// <summary>
        /// 添加新基因
        /// 根据xenoType的内源性添加谱系/异种基因
        /// </summary>
        public static void AddNewXenotypeGenes(Pawn target, XenotypeDef xenotype)
        {
#if DEBUG
            Log.Message("flm: Add xenogene: " + xenotype.label);
#endif

            foreach (GeneDef gene in xenotype.AllGenes)
            {
                if (gene.defName == DefsOf.Flm_MorphsE.defName)
                {
                    continue;
                }
#if DEBUG
                Log.Message("flm: Add gene: " + gene.label);
#endif
                target.genes.AddGene(gene, xenogene: true);
            }
        }


        /// <summary>
        /// 移除旧的异种基因
        /// </summary>
        public static void CleanOldXenotype(Pawn target)
        {
            for (int num = target.genes.Xenogenes.Count - 1; num >= 0; num--)
            {
                target.genes.RemoveGene(target.genes.Xenogenes[num]);
            }
        }
    }
}
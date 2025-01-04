using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class CompAbilityReimplantXeno : CompAbilityEffect
    {

        private bool isXenoSelected = false;

        public CompProperties_AbilityReimplantXeno Props
        {
            get
            {
                return (CompProperties_AbilityReimplantXeno)this.props;
            }
        }
        // private new CompProperties_AbilityReimplantXeno Props => (CompProperties_AbilityReimplantXeno)props;

        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            return base.GetPreCastActions();
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null)
            {
                Log.Message("targetPawn is null");
                return;
            }

            // 获取技能施放者
            Pawn caster = parent.pawn;
#if DEBUG
            Log.Message("caster: " + caster.Name);
            Log.Message("targetPawn: " + targetPawn.Name);
#endif

            List<FloatMenuOption> selectXenoMenu = new List<FloatMenuOption>();

            // 从施放者的基因中寻找可用的基因组
            Dictionary<string, XenotypeDef> xenoGenes = Utils.GetGeneXenotypes(caster);

            if (xenoGenes.Count == 0)
            {
                // 在左上角弹出消息
                Messages.Message("NoXenotype".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            // 绘制菜单
            foreach (var xeno_ in xenoGenes)
            {
                var xeno = xeno_;
                if (xeno.Value.defName == XmlDefs.Flm_FacelessMan.defName)
                {
                    continue;
                }

                var opt = new FloatMenuOption(xeno.Key, () =>
                {
                    if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
                    {
#if DEBUG
                        Log.Message("caster.genes.Xenotype.soundDefOnImplant: " + caster.genes.Xenotype.soundDefOnImplant);
#endif  
                        caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(caster));
                    }
                    ReimplantXenotype(caster, targetPawn, xeno.Value);
                    this.isXenoSelected = true;
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


            // 显示 FloatMenu
            Find.WindowStack.Add(menu);
        }

        public static void ReimplantXenotype(Pawn caster, Pawn target, XenotypeDef xenotype)
        {
            // 清除旧基因
            CleanOldGenes(target, xenotype);

            target.genes.xenotypeName = xenotype.label;
            target.genes.iconDef = Utils.GetXenotypeIcon(xenotype);
            

            // 设置新基因
            AddNewGenes(target, xenotype);
            // 设置异种基因
            target.genes.SetXenotype(xenotype);
            

            // 添加基因不稳定hediff
            target.health.AddHediff(XmlDefs.Flm_GeneticInstability);
            // 添加基因不稳定hediff
            // SetExtractGermline(caster);
            // 更新异种基因复制
            GeneUtility.UpdateXenogermReplication(target);
            Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
            if (firstHediffOfDef != null)
            {
                // 找到HediffComp_Disappears组件, 设置持续时间2天
                firstHediffOfDef.TryGetComp<HediffComp_Disappears>().ticksToDisappear = GenDate.TicksPerDay;
            }


        }

        /// <summary>
        /// 移除旧基因
        /// 根据xenoType的内源性移除谱系/异种基因
        /// </summary>
        public static void CleanOldGenes(Pawn target, XenotypeDef xenotype)
        {
            // 获取谱系基因/异种基因flag
            bool isEndotype = xenotype.inheritable;
            if (isEndotype) // 根据flag移除谱系/异种基因
            {
                for (int num = target.genes.Endogenes.Count - 1; num >= 0; num--)
                {
                    target.genes.RemoveGene(target.genes.Endogenes[num]);
                }
            }
            else
            {
                for (int num = target.genes.Xenogenes.Count - 1; num >= 0; num--)
                {
                    target.genes.RemoveGene(target.genes.Xenogenes[num]);
                }
            }
        }

        /// <summary>
        /// 添加新基因
        /// 根据xenoType的内源性添加谱系/异种基因
        /// </summary>
        public static void AddNewGenes(Pawn target, XenotypeDef xenotype)
        {
            foreach (GeneDef gene in xenotype.AllGenes)
            {
                target.genes.AddGene(gene, xenogene: !xenotype.inheritable);
            }
        }

        /// <summary>
        /// 为一个Pawn添加异种基因复制hediff, 并设置基因丢失冲击hediff
        /// 如果Pawn已经处于基因丢失状态, 则直接死亡
        /// </summary>
        public static void SetExtractGermline(Pawn caster, int overrideDurationTicks = -1)
        {
            caster.health.AddHediff(XmlDefs.Flm_GeneLossDizzy);
            if (GeneUtility.PawnWouldDieFromReimplanting(caster))
            {
                caster.genes.SetXenotype(XenotypeDefOf.Baseliner);
            }
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, caster);
            if (overrideDurationTicks > 0)
            {
                hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
            }
            caster.health.AddHediff(hediff);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return base.Valid(target, throwMessages);
            }
            if (pawn.IsQuestLodger())
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (!parent.pawn.genes.Endogenes.Any())
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_MessagePawnHasNoGermline".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }

            // if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
            // {
            //     if (throwMessages)
            //     {
            //         Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
            //     }
            //     return false;
            // }
            return base.Valid(target, throwMessages);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        {
            if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
            {
                return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
            }
            return null;
        }


        // public static void ReimplantGermline(Pawn caster, Pawn dest, XenotypeDef targetXenotype)
        // {

        //     // QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));

        //     List<GeneDef> xenogenesToPreserve = new List<GeneDef>();
        //     foreach (Gene gene in dest.genes.Xenogenes)
        //     {
        //         xenogenesToPreserve.Add(gene.def);
        //     }

        //     // TODO ->

        //     List<Gene> adjustedList = new List<Gene>();

        //     foreach (Gene endogene in caster.genes.Endogenes)
        //     {
        //         if (endogene.def.defName.Contains("VRE_Morphs"))
        //         {
        //             if (endogene.Active)
        //             {
        //                 adjustedList.Add(endogene);
        //             }
        //         }
        //         else adjustedList.Add(endogene);
        //     }


        //     foreach (Gene endogene in adjustedList)
        //     {
        //         dest.genes.AddGene(endogene.def, xenogene: false);

        //     }

        //     foreach (GeneDef xenogene in xenogenesToPreserve)
        //     {


        //         dest.genes.AddGene(xenogene, xenogene: true);

        //     }
        //     if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
        //     {
        //         caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(dest));
        //     }
        //     dest.health.AddHediff(XmlDefs.Flm_GeneticInstability);
        //     SetExtractGermline(caster);
        //     GeneUtility.UpdateXenogermReplication(dest);

        // }
    }
}
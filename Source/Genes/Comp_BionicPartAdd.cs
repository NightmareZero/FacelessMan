using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class CompProperties_BionicPartAdd : GeneExtCompProp
    {
        // 仿生体部件
        public HediffDef hediffDef;

        // 安装到的位置 (覆盖)
        public BodyPartDef bodyPartDef;

        // 安装数量
        public int installCount = 1;

        // 移除基因时恢复部位
        public bool recoverAfterRemoved = true;

        // 安装到所有对应部位
        public bool InstallToAll => installCount < 0;
        
        public CompProperties_BionicPartAdd()
        {
            compClass = typeof(GeneExtComp_BionicPartAdd);
        }
    }

    public class GeneExtComp_BionicPartAdd : GeneExtComp
    {
        public CompProperties_BionicPartAdd Props => (CompProperties_BionicPartAdd)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
        }

        public override void CompPostPostAdd()
        {
            
            base.CompPostPostAdd();

            // 获取安装部位
            var installBodyPartDef = Props.hediffDef.defaultInstallPart;
            if (Props.bodyPartDef != null)
            {
                installBodyPartDef = Props.bodyPartDef;
            }
            if (installBodyPartDef == null)
            {
                Log.Error("GeneExtComp_BionicPartAdd: No body part to install.");
                return;
            }

            // 获取可安装的部位
            var installableBodyParts = Pawn.health.hediffSet.GetNotMissingParts().ToList().FindAll(part => part.def == installBodyPartDef && !Pawn.health.hediffSet.HasHediff(Props.hediffDef, part));
            if (installableBodyParts.Count <= 0)
            {
                Log.Error("GeneExtComp_BionicPartAdd: No installable body part.");
                return;
            }
Log.Message("!GeneExtComp_BionicPartAdd: CompPostPostAdd, installableBodyParts: " + installableBodyParts.Count);
            // 进行安装
            if (Props.InstallToAll)
            {
                Log.Message("!GeneExtComp_BionicPartAdd: CompPostPostAdd, installToAll ");
                foreach (var bodyPart in installableBodyParts)
                {
                    InstallBionicPart(bodyPart);
                }
            }
            else
            {
                Log.Message("!GeneExtComp_BionicPartAdd: CompPostPostAdd, installCount: " + Props.installCount);
                for (int i = 0; i < Props.installCount; i++)
                {
                    if (installableBodyParts.Count <= 0)
                    {
                        break;
                    }
                    var bodyPart = installableBodyParts.RandomElement();
                    InstallBionicPart(bodyPart);
                    installableBodyParts.Remove(bodyPart);
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (Props.recoverAfterRemoved)
            {
                foreach (var hediff in Pawn.health.hediffSet.hediffs)
                {
                    if (hediff.def == Props.hediffDef && hediff.Part != null)
                    {
                        RecoverBionicPart(hediff.Part);
                    }
                }
            }
        }

        private void InstallBionicPart(BodyPartRecord bodyPart)
        {
            if (bodyPart != null)
            {
                Pawn.health.AddHediff(Props.hediffDef, bodyPart);
            }
        }

        private void RecoverBionicPart(BodyPartRecord bodyPart)
        {
            if (bodyPart != null)
            {
                Pawn.health.RestorePart(bodyPart);
            }
        }
    }
}
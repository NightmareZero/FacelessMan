using RimWorld;
using Verse;
using System.Reflection;

namespace NzFaceLessManMod
{
    public class CompAbilityCoagulateNoCheck : CompAbilityEffect_Coagulate
    {
        public new CompProperties_AbilityCoagulate Props => (CompProperties_AbilityCoagulate)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Pawn pawn = target.Pawn;
            if (pawn != null && !AbilityUtility.ValidateHasTendableWound(pawn, false, parent))
            {
                return;
            }
            base.Apply(target, dest);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            // 调用祖父类 CompAbilityEffect 的 Valid 方法，跳过父类 CompAbilityEffect_Coagulate 的检查
            return CallBaseBaseValid(target, throwMessages);
        }

        private bool CallBaseBaseValid(LocalTargetInfo target, bool throwMessages)
        {
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                if (!Props.canTargetBaby && !AbilityUtility.ValidateMustNotBeBaby(pawn, throwMessages, parent))
                {
                    return false;
                }

                if (!Props.canTargetBosses && pawn.kindDef.isBoss)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
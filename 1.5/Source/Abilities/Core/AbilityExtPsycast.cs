using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{
    public class AbilityExtPsycast : Psycast
    {
        // : public override void AbilityTick()


        public virtual void BeforeStartCooldown(ref int ticks)
        {
        }

        public virtual void BeforeNotify_GroupStartedCooldown(AbilityGroupDef group,ref int ticks)
        {
        }

        public virtual void AfterActivate(GlobalTargetInfo? target)
        { 
         
        }

        public virtual void AfterActivate(LocalTargetInfo? target, LocalTargetInfo? dest)
        {
        }

        public override bool Activate(GlobalTargetInfo target)
        {
            base.Activate(target);
            AfterActivate(target);
            return true;
        }

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Activate(target, dest);
            AfterActivate(target, dest);
            return true;
        }




        public AbilityExtPsycast(Pawn pawn) : base(pawn)
        {
        }

        public AbilityExtPsycast(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Linq;

namespace NzFaceLessManMod
{

    public class HediffComp_LinkOther : HediffComp_Link
    {
        public override bool CompShouldRemove
        {
            get
            {
                // 覆盖Base，不调用

                if (other == null)
                {
                    return true;
                }

                if (Props.requireLinkOnOtherPawn)
                {
                    if (!(other is Pawn pawn))
                    {
                        Log.Error("HediffComp_Link requires link on other pawn, but other thing is not a pawn!");
                        return true;
                    }

                }

                return false;
            }
        }

        public override void CompPostMake()
        {
            this.drawConnection = false; // 不绘制连接线(防止跨地图出问题)
            base.CompPostMake();
        }
    }
}
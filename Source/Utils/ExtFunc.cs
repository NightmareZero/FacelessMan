using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace NzFaceLessManMod
{
    /// <summary>
    /// 所有提供 this 方法的
    /// </summary>
    public static partial class Utils
    {

        /// <summary>
        /// 尝试获取链接的另一个Pawn
        /// </summary>
        /// <param name="hediff"></param>
        /// <param name="otherPawn"></param>
        /// <returns></returns>
        public static bool TryGetCompLinkedOtherPawn(this HediffWithComps hediff, out Pawn otherPawn)
        {
            otherPawn = null;

            HediffComp_Link link = hediff.TryGetComp<HediffComp_Link>();
            if (link == null || link.OtherPawn == null)
            {
                return false;
            }

            otherPawn = link.OtherPawn;
            return true;
        }
    }
}
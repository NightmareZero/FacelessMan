using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{

    public interface IGeneChangeListener
    {
        /// <summary>
        /// 通知基因变化
        /// </summary>
        /// <param name="gene"></param>
        /// <param name="action">
        // 1. 新增
        // 0. 变更
        // -1. 移除
        // </param>
        void Notify_OnGeneChange(Gene gene, int action);
    }
}
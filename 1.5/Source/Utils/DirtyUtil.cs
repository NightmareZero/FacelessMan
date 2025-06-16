using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace NzFaceLessManMod
{
    public enum DirtyCondition
    {
        None = 0,
        OnAdd = 1,
        OnRemove = 2,
        OnTick = 3,
    }

    public enum DirtyTrigger
    {
        None = 0,
        Hediff = 1,
        Gene = 2,
        Thing = 3,
    }
    public interface ISetDirty
    {
        void SetDirty(DirtyTrigger trigger = DirtyTrigger.None, DirtyCondition condition = DirtyCondition.None, object obj = null);
    }
}
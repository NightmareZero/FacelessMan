using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NzFaceLessManMod
{
    public class Hediff_MindWormMaster : HediffWithComps
    {
        public HashSet<Hediff> mindWorms = new HashSet<Hediff>(); // 心灵蠕虫列表

        public HashSet<Pawn> slaves = new HashSet<Pawn>(); // 奴隶列表

        public bool addWorm(Hediff worm)
        {
            if (mindWorms.Contains(worm))
            {
                Log.Error("MindWormMaster: addWorm failed, worm already exists in mindWorms list.");
                mindWorms.TryGetValue(worm, out var existingWorm);
                if (existingWorm.pawn != null)
                {
                    Log.Error("MindWormMaster: Existing worm master is " + existingWorm.pawn.Name.ToStringShort);
                }
                else
                {
                    mindWorms.Remove(existingWorm); // 移除已存在的蠕虫
                    mindWorms.Add(worm); // 添加新的蠕虫
                }
                return false;
            }
            mindWorms.Add(worm);
            if (worm.pawn != null && !slaves.Contains(worm.pawn))
            {
                slaves.Add(worm.pawn); // 添加到奴隶列表
            }

            return true;
        }

        public bool delWorm(Hediff worm)
        {
            if (!mindWorms.Contains(worm))
            {
                Log.Error("MindWormMaster: delWorm failed, worm not found in mindWorms list.");
                return false;
            }
            mindWorms.Remove(worm);
            if (worm.pawn != null)
            {
                slaves.Remove(worm.pawn); // 从奴隶列表中移除
            }
            else
            {
                updateSlavesFromMindWorms(); // 更新奴隶列表
            }


            return true;
        }

        private void updateSlavesFromMindWorms()
        {
            slaves.Clear();
            foreach (var worm in mindWorms.ToList())
            {
                if (worm != null && worm.pawn != null)
                {
                    slaves.Add(worm.pawn); // 添加到奴隶列表
                }
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();

            foreach (var worm in mindWorms.ToList())
            {
                worm.pawn?.health?.hediffSet?.GetHediffComps<HediffComp_MindWormSlave>()?.ToList()
                .ForEach(x => x.DoLostMaster()); // 失去主人时，调用Slave的DoLostMaster方法
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mindWorms, "mindWorms", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                updateSlavesFromMindWorms();
            }
        }
    }
}
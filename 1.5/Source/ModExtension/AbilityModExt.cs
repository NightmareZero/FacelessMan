using System.Collections.Generic;
using System.Dynamic;
using RimWorld;
using Verse;

namespace NzFaceLessManMod
{
    public class AbilityDefExt : DefModExtension
    {
        // 充能加成属性
        public List<StatModifier> chargeAddStatModifiers = new List<StatModifier>();

        // 冷却速度加成属性
        public List<StatModifier> cooldownAddStatModifiers = new List<StatModifier>();

        // 范围加成属性
        public List<StatModifier> areaAddStatModifiers = new List<StatModifier>();
        // 范围加成属性乘数
        public float areaAddStatMultiplier = 1f;
    }
}
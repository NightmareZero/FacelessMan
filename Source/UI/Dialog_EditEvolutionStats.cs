using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace NzFaceLessManMod
{
    // TODO: 重构
    // [HotSwappable]
    [StaticConstructorOnStartup]
    public static class Dialog_EditEvolutionStats
    {
        public static readonly IntRange GenelineStatRange = new IntRange(-20, 5);
        private struct IconConf
        {
            public string labelKey;

            public string descKey;

            public Texture2D icon;

            public IconConf(string labelKey, string descKey, Texture2D icon)
            {
                this.labelKey = labelKey;
                this.descKey = descKey;
                this.icon = icon;
            }
        }

        private static float cachedWidth;

        public static readonly CachedTexture EvolutionTex = new CachedTexture("UI/Icons/Abilities/ViewGenes");

        private static readonly IconConf iconConf = new IconConf("VRE_Evolutions", "VRE_EvolutionsDesc", EvolutionTex.Texture);

        private static Dictionary<string, string> truncateCache = new Dictionary<string, string>();
        private static float MaxLabelWidth()
        {
            return Text.CalcSize(iconConf.labelKey.Translate()).x;
        }

        public static float HeightForBiostats()
        {
            float num = Text.LineHeight * 2f;
            return num;
        }

        public static void Draw(Rect rect, int evolution, int evolutionLimit)
        {
            float num2 = MaxLabelWidth();
            float num3 = rect.height;
            GUI.BeginGroup(rect);
            // 绘制图标
            Rect position = new Rect(0f, (num3 - 22f) / 2f, 22f, 22f);
            Rect rect2 = new Rect(position.xMax + 4f, 0f, num2, num3);
            Rect rect3 = new Rect(0f, 0f, rect.width, num3);
            Widgets.DrawLightHighlight(rect3);
            TooltipHandler.TipRegion(rect3, iconConf.descKey.Translate());
            GUI.DrawTexture(position, iconConf.icon);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, iconConf.labelKey.Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            // 绘制文本
            float num4 = rect2.xMax + 4f;
            string text = $"{evolution} / {evolutionLimit}";
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(num4, 0f, 180f, num3), text); // 调整宽度以适应两个数字
            Text.Anchor = TextAnchor.UpperLeft;
            float width = rect.width - num4 - 180f - 4f; // 调整宽度
            Rect rect4 = new Rect(num4 + 180f + 4f, 0f, width, num3); // 调整位置
            if (rect4.width != cachedWidth)
            {
                cachedWidth = rect4.width;
                truncateCache.Clear();
            }
            GUI.EndGroup();
        }

        public static readonly SimpleCurve PowerEfficiencyToPowerDrainFactorCurve = new SimpleCurve
        {
            new CurvePoint(-20f, 6.0f),
            new CurvePoint(0f, 1f),
            new CurvePoint(5f, 0.5f)
        };

    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace NzFaceLessManMod
{
    public class ThingComp_DamageIgnore : ThingComp
    {
        public float chance = 0.34f; // 34% chance to ignore damage

        private static Material bubbleMat;
        private static Material BubbleMat
        {
            get
            {
                if (bubbleMat == null)
                {
                    bubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, Color.white);
                }
                return bubbleMat;
            }
        }

        private float minDrawSize = 1.2f;

        private float maxDrawSize = 1.55f;

        private Vector3 impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(0f);

        public void AddChance(float value)
        {
            chance = Mathf.Clamp(chance + value, 0f, 1f);
            if (chance <= 0.01f)
            {
                PawnOwner?.AllComps.Remove(this);
            }
        }

        public ThingComp_DamageIgnore WithParent(Pawn pawn)
        {
            parent = pawn;
            return this;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref parent, "parent");
            Scribe_Values.Look(ref chance, "chance", 0.5f); // 默认值为 0.5
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(ref dinfo, out absorbed);

            // 如果 Chance 大于随机数，则忽略伤害
            if (Rand.Value < chance)
            {
                impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                Vector3 loc = PawnOwner.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
                absorbed = true; // 忽略伤害
                dinfo.SetAmount(0); // 设置伤害为 0
#if DEBUG
                Log.Message($"[NzFaceLessManMod] Damage ignored: {dinfo.Def} on {PawnOwner.Name} with chance {chance}");
#endif
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            doDraw();

        }

        protected Pawn PawnOwner
        {
            get
            {
                if (parent is Apparel apparel)
                {
                    return apparel.Wearer;
                }

                if (parent is Pawn result)
                {
                    return result;
                }

                return null;
            }
        }

        private void doDraw()
        {
            // 当是征召状态时
            if (PawnOwner == null || !PawnOwner.Spawned || PawnOwner.Dead || PawnOwner.Map == null || !PawnOwner.Drafted)
            {
                return;
            }

            float num = Mathf.Lerp(minDrawSize, maxDrawSize, 0);
            Vector3 drawPos = PawnOwner.Drawer.DrawPos;
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            int num2 = Find.TickManager.TicksGame - Random.Range(0, 600);
            if (num2 < 8)
            {
                float num3 = (float)(8 - num2) / 8f * 0.05f;
                drawPos += impactAngleVect * num3;
                num -= num3;
            }

            float angle = Rand.Range(0, 360);
            Vector3 s = new Vector3(num, 1f, num);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);

        }
    }
}
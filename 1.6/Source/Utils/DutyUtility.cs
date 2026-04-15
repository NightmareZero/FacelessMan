using Verse;
using Verse.AI;

namespace FacelessMan
{
    public static class DutyUtility
    {
        /// <summary>
        /// 给Pawn分配指定的Duty
        /// </summary>
        /// <param name="pawn">要分配Duty的Pawn</param>
        /// <param name="dutyDef">Duty定义</param>
        /// <param name="focus">Duty的焦点位置</param>
        /// <param name="radius">Duty的半径范围</param>
        /// <returns>是否成功分配Duty</returns>
        public static bool AssignDuty(this Pawn pawn, DutyDef dutyDef, IntVec3? focus = null, float radius = -1f)
        {
            if (pawn == null || dutyDef == null)
            {
                Log.Error("AssignDuty: pawn or dutyDef is null");
                return false;
            }
            
            if (pawn.Dead || pawn.Downed || !pawn.Spawned)
            {
                Log.Warning($"AssignDuty: Pawn {pawn.Label} is not suitable for duty assignment");
                return false;
            }
            
            var duty = new PawnDuty(dutyDef);
            
            if (focus.HasValue)
            {
                duty.focus = new LocalTargetInfo(focus.Value);
            }
            
            if (radius > 0)
            {
                duty.radius = radius;
            }
            
            pawn.mindState.duty = duty;
            return true;
        }
        
        /// <summary>
        /// 给Pawn分配攻击职责
        /// </summary>
        /// <param name="pawn">要分配职责的Pawn</param>
        /// <param name="targetAcquireRadius">目标获取半径</param>
        /// <param name="targetKeepRadius">目标保持半径</param>
        /// <returns>是否成功分配职责</returns>
        public static bool AssignAssaultDuty(this Pawn pawn, float targetAcquireRadius = 65f, float targetKeepRadius = 72f)
        {
            // 检查是否有FleshbeastAssault职责
            var assaultDutyDef = DutyDefOf.FleshbeastAssault;
            if (assaultDutyDef == null)
            {
                Log.Error("AssignAssaultDuty: FleshbeastAssault duty not found");
                return false;
            }
            
            // 创建自定义职责定义
            var customDuty = new PawnDuty(assaultDutyDef);
            
            // 这里可以根据需要修改职责的属性
            // 注意：直接修改DutyDef可能会影响其他pawn，所以建议创建自定义DutyDef
            
            pawn.mindState.duty = customDuty;
            return true;
        }
        
        /// <summary>
        /// 批量给一组Pawn分配相同的Duty
        /// </summary>
        /// <param name="pawns">Pawn集合</param>
        /// <param name="dutyDef">Duty定义</param>
        /// <param name="focus">Duty的焦点位置</param>
        /// <param name="radius">Duty的半径范围</param>
        /// <returns>成功分配的Pawn数量</returns>
        public static int AssignDutyToGroup(this IEnumerable<Pawn> pawns, DutyDef dutyDef, IntVec3? focus = null, float radius = -1f)
        {
            int count = 0;
            foreach (var pawn in pawns)
            {
                if (pawn.AssignDuty(dutyDef, focus, radius))
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// 检查Pawn是否适合分配Duty
        /// </summary>
        /// <param name="pawn">要检查的Pawn</param>
        /// <returns>是否适合分配Duty</returns>
        public static bool IsSuitableForDuty(this Pawn pawn)
        {
            return pawn != null && !pawn.Dead && !pawn.Downed && pawn.Spawned && pawn.RaceProps.intelligence >= Intelligence.ToolUser;
        }
        
        /// <summary>
        /// 清除Pawn的Duty
        /// </summary>
        /// <param name="pawn">要清除Duty的Pawn</param>
        public static void ClearDuty(this Pawn pawn)
        {
            if (pawn != null)
            {
                pawn.mindState.duty = null;
            }
        }
        
        /// <summary>
        /// 获取Pawn当前的Duty
        /// </summary>
        /// <param name="pawn">要获取Duty的Pawn</param>
        /// <returns>当前的Duty，如果没有则返回null</returns>
        public static PawnDuty GetCurrentDuty(this Pawn pawn)
        {
            return pawn?.mindState.duty;
        }
        
        /// <summary>
        /// 检查Pawn是否有指定类型的Duty
        /// </summary>
        /// <param name="pawn">要检查的Pawn</param>
        /// <param name="dutyDef">要检查的Duty定义</param>
        /// <returns>是否有指定类型的Duty</returns>
        public static bool HasDuty(this Pawn pawn, DutyDef dutyDef)
        {
            return pawn?.mindState.duty?.def == dutyDef;
        }
    }
}
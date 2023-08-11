using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 거리를 판단하기 위한 디시전
    /// 해당 거리 이내이면 True를 반환한다.
    /// 시야를 체크하는 것과는 다르다는 것에 주의할 것
    /// </summary>
    // 판단 결과
    // AimTarget 설정
    // TargetInSight : true
    // PersonalTarget : AimTarget 위치
    // @To-Do
    // AimTarget Lost decision
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Focus")]
    public class FocusDecision : Decision
    {
        public enum Sense
        {
            Near, // 매우 가까움, 눈치 채지 못할 수가 없음
            Perception, // 인지 가능 범위
            View, // 시야 판단 거리
            CombatSpacing,
            RepositionThreshold,
        }

        [Tooltip("거리를 판단하기 위한 거리 분류")]
        public Sense SenseType;

        private bool TargetHandler(StateController controller, bool hasTargets, Collider[] targetInRadius)
        {
            foreach(Collider collider in targetInRadius)
            {
                if(!controller.BlockedSight(collider.transform.position))
                {
                    controller.AimTarget = collider.transform;
                    controller.TargetInSight = true;
                    controller.PersonalTarget = controller.AimTarget.position;
                    return true;
                }
            }

            return false;
        }

        public override bool Decide(StateController controller)
        {
            // Near이 아닐 경우 피격을 당할 경우 바로 인지
            // Near일 경우는 거리만 체크
            // FeelAlert를 True로 설정할 때 AimTarget도 같이 설정해 준다.
            return (SenseType != Sense.Near && controller.Variables.FeelAlert && !controller.BlockedSight(controller.AimTarget.position)) ||
                Decision.CheckTargetsInRadius(controller, GetRadius(controller), TargetHandler);
        }

        private float GetRadius(StateController controller)
        {
            return SenseType switch
            {
                // Sense.Near => controller.
                Sense.Near => controller.GeneralStats.NearRadius,
                Sense.Perception => controller.GeneralStats.PerceptionRadius,
                Sense.View => controller.GeneralStats.NearRadius,
                Sense.CombatSpacing => controller.CombatSpacing,
                Sense.RepositionThreshold => controller.RepositionThreshold,
                _ => controller.GeneralStats.NearRadius,
            };
        }
    }
}

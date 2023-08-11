using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 공격 위치로 이동하는 Action
    /// </summary>
    // Expect Behaviour
    // 1. 한 번 잡은 target을 향해 이동해야 한다. 중간에 다른 target을 설정하지 않는다.
    // 2. 경로가 존재해야 한다.
    // 단순하게 Chase가 아니라 Move로 추상화해서 사용할 수도 있을 것 같다.
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Chase")]
    public class ChaseAction : Action
    {
        // 이게 action에서 수행되는 것이 맞는가?
        // Decision 쪽도 생각해 볼 것
        // 아니명 Decision에 위임할 수도 있긴 한데 이거는 거리라서 조금 애매하네
        public enum CloseInterval
        {
            AttackRange,
            CombatSpacing,
        }

        public CloseInterval IntervalType;

        public override void OnReadyAction(StateController controller)
        {
            // @To-Do
            // 원거리 공격일 수도 있으므로
            // Attack Range를 따로 받아와야 한다.
            // 추후에 AttackController와 연계해서 처리할 것
            // 지금은 FSM 전이 위주로 작업할 것
            controller.Nav.speed = controller.GeneralStats.ChaseSpeed;            
            controller.Nav.stoppingDistance = GetDistance(controller);
        }

        public override void Act(StateController controller)
        {
            // 이 행동을 따로 분리해서 독립적으로 사용할 수 있을 것 같다.
            float dist = Vector3.Distance(controller.transform.position, controller.PersonalTarget);
            if(dist < GetDistance(controller))
            {
                controller.Nav.destination = controller.transform.position;
            }
            else
            {
                controller.Nav.destination = controller.PersonalTarget;
            }
        }

        private float GetDistance(StateController controller)
        {
            return IntervalType switch
            {
                CloseInterval.AttackRange => controller.AttackRange,
                CloseInterval.CombatSpacing => controller.CombatSpacing,
                _ => controller.AttackRange,
            };
        }
    }
}

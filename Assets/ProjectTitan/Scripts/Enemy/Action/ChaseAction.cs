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
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Chase")]
    public class ChaseAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.speed = controller.GeneralStats.ChaseSpeed;            
            controller.Nav.stoppingDistance = controller.AttackRange;
        }

        public override void Act(StateController controller)
        {
            float dist = Vector3.Distance(controller.transform.position, controller.AimTarget.position);
            if(dist < controller.AttackRange)
            {
                controller.Nav.destination = controller.transform.position;
            }
            else
            {
                controller.Nav.destination = controller.AimTarget.position;
            }
        }
    }
}

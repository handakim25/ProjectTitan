using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 공격 대기 시간에 거리를 유지하는 행동
    /// </summary>
    // 행동 전략
    // 1. 거리가 멀어지면 다시 달라 붙는다.
    // 2. 거리가 너무 가까우면 뒤로 간다.
    // 3. 일정 시간 상태가 유지되면 좌우로 스탭을 밟는다.
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Combat dist maintain")]
    public class CombatDistMaintainAction : Action
    {
        public FocusDecision NearDecision;
        public ReachedPointDecision reachedPointDecision;
        
        public override void OnReadyAction(StateController controller)
        {
            controller.IsFocusTarget = true;
            controller.IsStraffing = true;
            controller.Nav.destination = controller.transform.position;
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsFocusTarget = false;
            controller.IsStraffing = false;
        }

        public override void Act(StateController controller)
        {
            var dist = controller.GetPersonalTargetDist();
            if(dist > controller.RepositionThreshold)
            {
                // Chase
                
            }
            else if(NearDecision.Decide(controller))
            {
                // Near go far
            }
        }
    }
}

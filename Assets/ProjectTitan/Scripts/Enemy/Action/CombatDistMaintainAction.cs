using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 공격 대기 시간에 거리를 유지하는 행동
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Combat dist maintain")]
    public class CombatDistMaintainAction : Action
    {
        public FocusDecision NearDecision;
        public ReachedPointDecision reachedPointDecision;
        
        public override void OnReadyAction(StateController controller)
        {
            controller.IsFocusTarget = true;
            controller.Nav.destination = controller.transform.position;
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsFocusTarget = false;
        }

        public override void Act(StateController controller)
        {
            var dist = controller.GetPersonalTargetDist();
            if(dist > controller.RepositionThreshold)
            {
                
            }
            else if(NearDecision.Decide(controller))
            {
                // Near go far
            }
        }
    }
}

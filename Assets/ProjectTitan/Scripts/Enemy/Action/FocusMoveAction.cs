using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// Repositioning Action
    /// 거리가 가까우면 뒤로 후진, 
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/FocusMove")]
    public class FocusMoveAction : Action
    {
        public FocusDecision nearDecision;

        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.destination = controller.transform.position;
            controller.IsFocusTarget = true;
            // controller.IsAligned = false;
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsFocusTarget = false;
        }

        public override void Act(StateController controller)
        {
            // controller.Nav.destination = controller.PersonalTarget;
            // controller.Nav.speed = 0f;
            // if(!controller.IsAligned)
            // {
            //     controller.Nav.destination = controller.PersonalTarget;
            //     controller.Nav.speed = 0f;
            //     if(controller.EnemyAnim.AngularSpeed == 0f)
            //     {
            //         controller.IsAligned = true;
            //         controller.Nav.destination = controller.AimTarget.position;
            //         controller.Nav.speed = controller.GeneralStats.ChaseSpeed;
            //     }
            // }
            // else
            // {

            // }
            
            // if close
            // move back
            // if too far
            // move close?

            float targetDist = Vector3.Distance(controller.transform.position, controller.PersonalTarget);
            if(targetDist > controller.RepositionThreshold)
            {
                // reposition
                Debug.Log($"repositioning");
            }
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Return Action")]
    public class ReturnAction : Action
    {
        public float RetrunStoppingDistance = 0f;
        
        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.destination = controller.Variables.ReturnPos;
            controller.Nav.speed = controller.GeneralStats.ReturnSpeed;
            controller.Nav.stoppingDistance  = RetrunStoppingDistance;

            // @To-Do
            // 무적설정
            
        }

        public override void OnDisableAction(StateController controller)
        {
            
        }

        public override void Act(StateController controller)
        {
            if(controller.Nav.remainingDistance <= controller.Nav.stoppingDistance && !controller.Nav.pathPending)
            {
                controller.Nav.destination = controller.transform.position;
            }
        }
    }
}

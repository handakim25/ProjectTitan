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
            controller.IsInvincible = true;
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsInvincible = false;
        }

        public override void Act(StateController controller)
        {
            if(HasArrived(controller))
            {
                controller.Nav.destination = controller.transform.position;
            }
        }
    }
}

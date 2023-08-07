using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Return Action")]
    public class ReturnAction : Action
    {

        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.destination = controller.Variables.ReturnPos;
            controller.Nav.speed = controller.GeneralStats.ReturnSpeed;
        }      
        
        public override void Act(StateController controller)
        {

        }
    }
}

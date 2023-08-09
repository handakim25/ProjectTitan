using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Reached Point")]
    public class ReachedPointDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            if(Application.isPlaying == false)
            {
                return false;
            }

            if(controller.Nav.remainingDistance <= controller.Nav.stoppingDistance &&
                !controller.Nav.pathPending && controller.Nav.desiredVelocity == Vector3.zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    // 일반적인 대기 상태
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public float PatrolStoppingDistance = 0f;

        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.stoppingDistance = PatrolStoppingDistance;
            controller.Variables.PatrolTimer = 0f;
            controller.Nav.speed = controller.GeneralStats.PatrolSpeed;
        }

        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            // If no waypoint, stay
            if(controller.PatrolWaypoints.Length == 0)
            {
                return;
            }

            // controller
            if(controller.Nav.remainingDistance <= controller.Nav.stoppingDistance && !controller.Nav.pathPending)
            {
                controller.Variables.PatrolTimer += Time.deltaTime;
                if(controller.Variables.PatrolTimer > controller.GeneralStats.PatrolWaitTime)
                {
                    controller.WaypointIndex = (controller.WaypointIndex + 1) % controller.PatrolWaypoints.Length;
                    controller.Variables.PatrolTimer = 0f;
                }
            }
            controller.Nav.destination = controller.PatrolWaypoints[controller.WaypointIndex].position;
        }
    }
}

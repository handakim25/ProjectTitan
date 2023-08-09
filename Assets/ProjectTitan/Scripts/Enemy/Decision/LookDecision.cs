using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Look")]
    public class LookDecision : Decision
    {
        private bool TargetHandler(StateController controller, bool hasTargets, Collider[] targetInRadius)
        {
            foreach(Collider collider in targetInRadius)
            {
                // Debug.Log($"Look Target : {collider.name}");
                Vector3 target = collider.transform.position;
                Vector3 dirToTarget = target - controller.transform.position;
                bool inFovCondition = Vector3.Angle(controller.transform.forward, dirToTarget) < controller.GeneralStats.ViewAngle / 2;
                if(inFovCondition && !controller.BlockedSight(controller.transform.position))
                {
                    controller.AimTarget = collider.transform;
                    controller.TargetInSight = true;
                    controller.PersonalTarget = controller.AimTarget.position;
                    return true;
                }
            }
            return false;
        }

        public override bool Decide(StateController controller)
        {
            controller.TargetInSight = false;
            return Decision.CheckTargetsInRadius(controller, controller.GeneralStats.ViewRadius, TargetHandler);
        }
    }
}

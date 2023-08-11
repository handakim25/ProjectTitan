using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// Target으로부터 멀어지는 행동
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Fallback")]
    public class FallbackAction : Action
    {

        public override void OnReadyAction(StateController controller)
        {
            controller.IsFocusTarget = true;
            // controller.Nav.speed = controller.GeneralStats.StrafTurnSpeed;
            // controller.Nav.stoppingDistance = 0f;
            controller.Nav.ResetPath();
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsFocusTarget = false;
        }

        public override void Act(StateController controller)
        {
            Vector3 retreatDir = controller.transform.position - controller.PersonalTarget;
            retreatDir.y = 0f;
            controller.Nav.velocity = 1f * retreatDir.normalized;
        }
    }
}

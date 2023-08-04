using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 공격 State에서 해당 목표에 따라 이동하는 Action
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/FocusMove")]
    public class FocusMoveAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.destination = controller.transform.position;
            controller.IsFocusTarget = true;
        }

        public override void Act(StateController controller)
        {
            controller.Nav.destination = controller.PersonalTarget;
            controller.Nav.speed = 0f;
            
        }
    }
}

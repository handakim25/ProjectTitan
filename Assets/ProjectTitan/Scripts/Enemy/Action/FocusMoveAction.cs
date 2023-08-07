using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    /// <summary>
    /// 공격에 따른 이동을 구현하기 위한 State
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/FocusMove")]
    public class FocusMoveAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            controller.Nav.destination = controller.transform.position;
            controller.IsFocusTarget = true;
            controller.IsAligned = false;
        }

        public override void Act(StateController controller)
        {
            controller.Nav.destination = controller.PersonalTarget;
            controller.Nav.speed = 0f;
            if(!controller.IsAligned)
            {
                controller.Nav.destination = controller.PersonalTarget;
                controller.Nav.speed = 0f;
                if(controller.EnemyAnim.AngularSpeed == 0f)
                {
                    controller.IsAligned = true;
                    controller.Nav.destination = controller.AimTarget.position;
                    controller.Nav.speed = controller.GeneralStats.ChaseSpeed;
                }
            }
            else
            {

            }
        }
    }
}

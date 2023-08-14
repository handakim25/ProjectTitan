using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Actions/Perform Attack")]
    public class PerformAttackAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            controller.IsFocusTarget = true;
            controller.IsAligned = false;
            controller.IsAttack = false;
            controller.EnemyAnim.ResetTrigger();
        }

        public override void OnDisableAction(StateController controller)
        {
            controller.IsFocusTarget = false;
            controller.IsAttack = false;

            controller.Variables.AttackEndTime = Time.time;
        }

        public override void Act(StateController controller)
        {
            // @To-Do 
            // 로직 버그 있음
            if(!controller.IsAligned && controller.EnemyAnim.AngularSpeed < 5f)
            {
                controller.IsAligned = true;
            }
            else
            {
                if(!controller.IsAttack)
                {
                    // Aim 동작이 있으면 Aim tlfgod
                    PerformAttack(controller);
                    controller.IsAttack = true;
                }
            }
        }

        private void PerformAttack(StateController controller)
        {
            // Do controller action
            controller.EnemyAnim.Animator.SetTrigger(AnimatorKey.Enemy.AttackTrigger);
        }
    }
}

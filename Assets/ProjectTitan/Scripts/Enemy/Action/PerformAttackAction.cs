using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    public class PerformAttackAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            
        }

        public override void Act(StateController controller)
        {
            controller.IsFocusTarget = true;

            if(CanAttack())
            {
                PerformAttack();    
            }
        }

        // 조준이 필요할 수도 있고 아니면 근접 공격일 수도 있다.
        private bool CanAttack()
        {
            // if aligned
            // perform
            return false;
        }

        private void PerformAttack()
        {

        }
    }
}

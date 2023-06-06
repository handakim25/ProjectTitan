using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    public class MeleeBehaviour : AttackBehaviour
    {
        // Attack Collider

        public override void OnEnter()
        {
            _controller.PlayerMove.Speed = 0f;

            // Play Animation
            _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex);
            _controller.Animator.SetTrigger(GetAnimTriggerParam());
        }
        
        // @refactor
        // Temp method
        public void ExecuteAttack()
        {

        }
    }
}

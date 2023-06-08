using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    public class MeleeComboBehaviour : AttackBehaviour
    {
        public override void OnEnter()
        {
            base.OnEnter();
            _controller.PlayerMove.Speed = 0f;

            _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex);
            _controller.Animator.SetTrigger(GetAnimTriggerParam());
        }

        public override void LocalUpdate()
        {
            if(_controller.IsGround)
            {
                _controller.PlayerMove.ResetFall();
            }
        }

        protected override void AttackPerformedHandler()
        {
            if(_controller.IsCurrentBehaviour(BehaviourCode))
            {
                // Check Combo
                _controller.Animator.SetBool(AnimatorKey.Player.HasCombo, true);
                _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex + 1);
            }
            else if(!CanAttack())
            {
                return;
            }

            _controller.RegisterBehaviour(BehaviourCode);
        }
    }
}

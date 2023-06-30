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

        // @Refactor
        // 추후에 AttackBehaviour로 옮겨서 General한 케이스에도 작동할 수 있도록 할 것
        protected override void AttackPerformedHandler()
        {
            if(_controller.IsCurrentBehaviour(BehaviourCode))
            {
                // Check Combo
                _controller.Animator.SetTrigger(GetAnimTriggerParam());
                // _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex + 1);
            }
            else if(!CanAttack())
            {
                return;
            }

            _controller.RegisterBehaviour(BehaviourCode);
        }
    }
}

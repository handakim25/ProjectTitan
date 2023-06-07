using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    public class MeleeComboBehaviour : AttackBehaviour
    {
        public override void OnEnter()
        {
            _controller.PlayerMove.Speed = 0f;

            _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex);
            _controller.Animator.SetTrigger(GetAnimTriggerParam());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan.Character.Player
{
    public class MeleeBehaviour : AttackBehaviour
    {
        // Attack Collider

        public override void OnEnter()
        {
            base.OnEnter();
            _controller.PlayerMove.Speed = 0f;

            // Play Animation
            _controller.Animator.SetInteger(GetAnimIndexParam(), _attackList[curAttackIndex].animIndex);
            _controller.Animator.SetTrigger(GetAnimTriggerParam());
        }

        public override void LocalUpdate()
        {
            if(_controller.IsGround)
            {
                _controller.PlayerMove.ResetFall();
            }
        }

        // @refactor
        // Melee와 통합시켜서 공통의 코드로 사용할 것
        // Callback when impact time.
        protected override void PerformAttack()
        {
            var attack = _attackList[0];
            LayerMask target = LayerMask.GetMask("Enemy", "Destructable");
            var colliders = attack.damageHitBox?.CheckOverlap(target) ?? new Collider[0];
            foreach(var collider in colliders)
            {
                Debug.Log($"Collider : {collider.name}");
                if(collider.TryGetComponent<UnitHealth>(out var targetHealth))
                {
                    targetHealth.TakeDamage(new Vector3(0, 0, 0), new Vector3(0, 0, 0), attack.damageFactor);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;
using Titan.Audio;

namespace Titan.Character.Player
{
    public class MeleeComboBehaviour : AttackBehaviour
    {
        // 0-base
        protected int _comboCount;

        public override void OnEnter()
        {
            base.OnEnter();
            _controller.PlayerMove.Speed = 0f;

            _controller.Animator.SetInteger(GetAnimIndexParam(), _animationIndex);
            _controller.Animator.SetTrigger(GetAnimTriggerParam());

            _comboCount = 0;
        }

        public override void OnExit()
        {
            _controller.Animator.ResetTrigger(GetAnimTriggerParam());
        }

        public override void LocalUpdate()
        {
            // @Think
            // Attack에서 Player Move를 너무 구체적으로 작업하는 것 같다.
            // 추가적인 추상화를 할 수 있을지 생각해 볼 것
            if(_controller.IsGround)
            {
                _controller.PlayerMove.ResetFall();
            }
        }

        public override void LocalFixedUpdate()
        {
            // 이거 왜 작성했지? 잘 기억 안 나네
            _controller.Animator.ResetTrigger(GetAnimTriggerParam());
        }

        // @Refactor
        // 추후에 AttackBehaviour로 옮겨서 General한 케이스에도 작동할 수 있도록 할 것
        protected override void AttackPerformedHandler()
        {
            if(!CanAttack())
            {
                return;
            }
            
            // 현재 상태에서 Attack 입력이 다시 들어왔을 경우
            if(_controller.IsCurrentBehaviour(BehaviourCode))
            {
                // Check Combo
                _controller.Animator.SetTrigger(GetAnimTriggerParam());
            }
            else
            {
                _controller.RegisterBehaviour(BehaviourCode);
            }
        }

        protected override void PerformAttack()
        {
            if(_comboCount >= _attackList.Count)
            {
                Debug.LogError($"Attack List is missing!");
                return;
            }
            var attack = _attackList[_comboCount++];
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

            SoundManager.Instance.PlayEffectSound((int)attack.sfx);
        }
    }
}

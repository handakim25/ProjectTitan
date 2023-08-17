using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy
{
    // Animator에 붙어 있다.
    // 관련 로직들 따로 정리하고 일단은 이 부분은 나중에 할 것
    // 현재는 FSM 구현을 우선으로 한다.
    public class EnemyAttackController : MonoBehaviour
    {
        private StateController _controller;

        private void Awake()
        {
            _controller = GetComponentInParent<StateController>();
        }

        public void PerformAttack()
        {
            
        }

        #region Animator Callback
        
        // Animation Event
        private void ExecuteAttack()
        {
            Debug.Log($"Enemy Attack");
        }
        
        #endregion Animator Callback
    }
}

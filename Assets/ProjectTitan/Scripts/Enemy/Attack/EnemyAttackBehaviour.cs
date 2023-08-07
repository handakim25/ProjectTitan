using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan.Character.Enemy
{
    public class EnemyAttackBehaviour : MonoBehaviour
    {
        private int _priority;
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private float _coolTime;
        private float _timer;
        [SerializeField] private int _animIndex;

        public int Priority => _priority;

        private void Update()
        {
            // Update Cooltime
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan
{
    [RequireComponent(typeof(ObjectHealth))]
    public class Destructable : MonoBehaviour
    {
        // reward
        // durablity

        Animator _animator;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            var health = GetComponent<ObjectHealth>();
            health.OnDeath.AddListener(() => Dead());
        }

        // 피격 방향 필요
        public void Hit()
        {

        }

        public void Dead()
        {
            Debug.Log($"Dead : {gameObject.name}");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
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
        }

        // 피격 방향 필요
        public void Hit()
        {

        }

        public void Dead()
        {

        }
    }
}

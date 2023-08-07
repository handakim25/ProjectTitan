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
        
        [SerializeField] private GameObject _deadVfx;
        [SerializeField] private GameObject _deadSfx;
        [SerializeField] private int _itemRewardCode;
        [SerializeField] private float _itemRewardWeight;

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
            if(_deadVfx != null)
            {
                Instantiate(_deadSfx, transform.position, Quaternion.identity);
            }
            if(_deadSfx != null)
            {
                // Play SFX
            }
            // Calculate item ratio
            // Instatntiate drop item
            
            Debug.Log($"Dead : {gameObject.name}");
            Destroy(gameObject);
        }
    }
}

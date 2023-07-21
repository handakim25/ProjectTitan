using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    public class ObjectHealth : UnitHealth
    {
        [Tooltip("How many attack is needed to destruct")]
        [SerializeField] private int _durability = 1;

        public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null)
        {
            if(isAlive == false)
            {
                return;
            }

            Debug.Log($"Got damage");
            _durability -= 1;
            if(_durability <= 0)
            {
                Dead();
            }
        }

        public override void Dead()
        {
            Debug.Log($"Dead");
            Destroy(gameObject);
        }
    }
}

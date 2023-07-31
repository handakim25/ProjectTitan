using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    public class EnemyHealth : UnitHealth
    {

        public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null)
        {
            Debug.Log($"Hit");
        }
    }
}

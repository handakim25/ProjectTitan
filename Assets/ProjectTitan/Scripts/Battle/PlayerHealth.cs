using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    public class PlayerHealth : UnitHealth
    {
        [SerializeField] private FloatVariable _playerHealth;
        [SerializeField] private FloatVariable _plyaerMaxHealth;

        [SerializeField] private float initHealth = 100f;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _playerHealth.Value = 100f;
            _plyaerMaxHealth.Value = 100f;
        }

        public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null)
        {
            throw new System.NotImplementedException();
        }
    }
}

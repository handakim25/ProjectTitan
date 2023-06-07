using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    // @Note
    // Scriptable Object로 고려
    // Editor 지원이나
    [System.Serializable]
    public class AttackData
    {
        public int animIndex;
        public int damageFactor;
        public Collider damageHitBox;
        public GameObject projectilePrefab;
        public GameObject sfx; // 추후에 변경
        public GameObject vfx; // 추후에 변경
    }
}

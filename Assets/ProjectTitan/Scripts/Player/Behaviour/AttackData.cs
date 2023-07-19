using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    // @Note
    // Attack은 2가지로 구분된다.
    // 1. 연출적인 부분
    // - SFX, VFX, Animation...
    // 2. 데미지 처리 부분
    // - Collider, Proejectile, hitbox
    // @Refactor
    // 데미지 처리 부분을 Data 형식으로 변경할 것
    [System.Serializable]
    public class AttackData
    {
        public AttackCollider damageHitBox;
        public GameObject projectilePrefab;
        public GameObject sfx; // 추후에 변경
        public GameObject vfx; // 추후에 변경
        public int animIndex;
        public int damageFactor;
    }
}

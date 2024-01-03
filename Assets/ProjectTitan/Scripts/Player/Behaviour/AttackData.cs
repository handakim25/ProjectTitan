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
        /// <summary>
        /// Attack 범위를 지정하는 Collider
        /// </summary>
        public AttackCollider damageHitBox;
        /// <summary>
        /// Projectile을 사용할 경우 생성할 Prefab
        /// </summary>
        public GameObject projectilePrefab;
        /// <summary>
        /// 공격 시에 사용할 Sfx
        /// </summary>
        public SoundList sfx = SoundList.None;
        public GameObject vfx; // 추후에 변경
        /// <summary>
        /// 재생할 Animation Index
        /// </summary>
        public int animIndex;
        /// <summary>
        /// 적용할 데미지 Factor
        /// </summary>
        public int damageFactor;
        // public bool knockbackForce;
        // public bool knockbackHeight;
        // public float defenderStunDuration;
    }
}

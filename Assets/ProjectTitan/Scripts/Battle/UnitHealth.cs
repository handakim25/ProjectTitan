using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.Battle
{
    /// <summary>
    /// 체력을 가진 오브젝트들의 기반 클래스
    /// 플레이어, Enemy, Object 등이 HP를 가질 수 있다.
    /// </summary>
    public abstract class UnitHealth : MonoBehaviour
    {
        // event
        // 1. heat
        // 2. die
        public UnityEvent OnHit;
        public UnityEvent OnDeath;

        protected bool isAlive = true;

        // 추가적인 공격 데이터가 필요할 수도 있다.
        // - Knockback 정보 : 방향, 힘, 시간
        // - 경직 정보 : 몬스터가 경직 저항치를 가지므로 몬스터가 처리해야 한다. 경직 파워를 지정해서 몬스터의 경직 저항치와 비교해서 처리
        // - 상태 이상 정보
        /// <summary>
        /// 공통적인 피해 함수
        /// </summary>
        /// <param name="location"></param>
        /// <param name="direction"></param>
        /// <param name="damage">Attacker에서 계산한 순수 데미지</param>
        /// <param name="bodyPart"></param>
        /// <param name="attacker">Attacker</param>
        public abstract void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null);

        public virtual void Dead()
        {
            OnDeath?.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    /// <summary>
    /// 체력을 가진 오브젝트들의 기반 클래스
    /// 플레이어, Enemy, Object 등이 HP를 가질 수 있다.
    /// </summary>
    public class UnitHealth : MonoBehaviour
    {
        public virtual void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null)
        {

        }
    }
}

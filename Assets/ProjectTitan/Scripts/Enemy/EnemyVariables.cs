using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy
{
    /// <summary>
    /// Enemy의 변수들을 정래해 두는 용도
    /// </summary>
    [System.Serializable]
    public class EnemyVariables
    {
        public bool FeelAlert; // 공격 받을 시에 활성화
        // Patrol State
        public float PatrolTimer; // GeneralStats.PatrolWaitTime을 체크하기 위한 변수
        // Return State
        public Vector3 ReturnPos;
        // Positioning State
        public float AttackTimer; // Attack State로 진입하기 위한 시간
        public float AttackWaitTime; // Attack State 대기 시간. 이 만큼 지나면 Repositioning 실행
        // Attack State
    }
}

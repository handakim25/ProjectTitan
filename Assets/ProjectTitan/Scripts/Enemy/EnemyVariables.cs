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
        public float AttackEndTime; // 공격이 끝난 시간
        public float RepositionWaitTime;
        // Attack State

        // Wait Decision
        public float WaitStartTime;
    }
}

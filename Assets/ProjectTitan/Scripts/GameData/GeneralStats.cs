using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy
{
    /// <summary>
    /// Enemy들이 공통적으로 사용하는 속성
    /// 전체적인 게임의 느낌을 한 번에 설정하도록 한다.
    /// </summary>
    [CreateAssetMenu(menuName = "Enemy/GeneralStats")]
    public class GeneralStats : ScriptableObject
    {
        [Header("Ptrol")]
        [Tooltip("Patrol 상태에서 대기 시간")]
        public float PatrolSpeed = 2f;
        [Tooltip("Patrol Point에 도착하고 대기하는 시가")]
        public float PatrolWaitTime = 2f;

        // 일부 값들은 유닛에 종속된 속성인 거 같기도 하다.
        // 추후에 확인할 것
        [Header("Battle")]
        [Tooltip("공격하기 위해 접근하는 속도")]
        public float ChaseSpeed = 5f;
        [Tooltip("거리 유지 상태에서의 속도")]
        public float PositioningSpeed = 3f;
        [Tooltip("공격 상태로 들어가기 전에 대기하는 시간")]
        public float AttackWaitDuration = 4f;
        [Tooltip("Reposition 상태에서 새로운 위치로 이동하는 대기 시간")]
        public float RepositionInterval = 2f;

        [Header("Return")]
        [Tooltip("최대 전투 거리")]
        public float MaxBattlDistance = 10f;
        [Tooltip("복귀 속도")]
        public float ReturnSpeed = 20f;

        [Header("Animation")]
        public float SpeedDampTime = 0.4f;
        public float AngularSpeedDampFactor = 20f;
        public float StrafTurnSpeed = 25f;
        public float MaxAngularSpeed = 180f;

        [Header("Sight")]
        [Range(0, 50)]
        [Tooltip("시야 거리")]
        public float ViewRadius = 10f;
        [Range(0, 360)]
        [Tooltip("시야 각도")]
        public float ViewAngle = 60f;
        [Range(0, 25)]
        [Tooltip("인지 거리, 각도에 상관 없이 판정")]
        public float PerceptionRadius = 5f;
        [Range(0, 10)]
        [Tooltip("가까운 거리, 장애물 관계 없이 판정")]
        public float NearRadius = 2.5f;

        [Header("Layer")]
        [Tooltip("적 대상 마스크")]
        public LayerMask targetMask;
        [Tooltip("장애물 마스크, 시야 판정에 사용")]
        public LayerMask obstacleMask;
    }
}

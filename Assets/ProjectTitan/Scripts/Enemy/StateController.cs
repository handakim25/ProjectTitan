using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Titan.Character.Enemy.FSM;

namespace Titan.Character.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateController : MonoBehaviour
    {
        public GeneralStats GeneralStats;

        // @To-Do
        // 지금은 직접 설정 중이지만 엑셀 파일에서 읽을 수 있도록 설정할 것
        // 필요 데이터가 굉장히 많다
        // 또한 일괄적으로 한 번에 수정될 필요도 있고

        public State currentState;
        public State remainState;

        private bool aiActive;
        public Transform[] PatrolWaypoints;
        [HideInInspector] public int WaypointIndex;

        [HideInInspector] public Transform AimTarget; // 현재 조준 상대
        [HideInInspector] public Vector3 PersonalTarget = Vector3.zero; // 현재 목표 지점, Action에 따라 작동 방법이 다르다.
        [HideInInspector] public bool TargetInSight; // Target을 발견했는가
        [HideInInspector] public bool IsFocusTarget; // 전투 상태 중에서 목표를 주시하는가. Straffing에 사용할 것
        [HideInInspector] public bool IsAligned;

        [HideInInspector] public float AttackRange => 2f; // 나중에 구현하면서 다른 곳으로 옮길 것

        // Cache and access from state
        [HideInInspector] public NavMeshAgent Nav;
        [HideInInspector] public EnemyAnimation EnemyAnim;
        [HideInInspector] public EnemyVariables Variables;
        
        private void Awake()
        {
            aiActive = true;

            Nav = GetComponent<NavMeshAgent>();
            EnemyAnim = gameObject.AddComponent<EnemyAnimation>();

            Debug.Assert(currentState, "State is not set");
            Debug.Assert(remainState, "State is not set");

            if(!currentState || !remainState)
            {
                aiActive = false;
            }
        }

        private void Update()
        {
            if(!aiActive)
            {
                return;
            }

            currentState.DoActions(this);
            currentState.CheckTransitions(this);
        }

        public void TransitionToState(State nextState, Decision decision)
        {
            if(nextState != remainState)
            {
                bool enteringCombat = !currentState.IsCombat && nextState.IsCombat;
                
                currentState = nextState;

                if(enteringCombat)
                {
                    Variables.ReturnPos = transform.position;
                }
            }
        }

        /// <summary>
        /// 시야 막힘 여부 확인
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns>True이면 막혀 있음, False일 경우 안 막혀 있음</returns>
        public bool BlockedSight(Vector3 targetPos)
        {
            Vector3 castOrigin = transform.position + Vector3.up * 1.5f;
            targetPos = targetPos + Vector3.up * 1.5f;
            Vector3 dirToTarget = targetPos - castOrigin;
            return Physics.Raycast(castOrigin, dirToTarget, out RaycastHit hit, dirToTarget.magnitude, GeneralStats.obstacleMask);
        }
    }
}

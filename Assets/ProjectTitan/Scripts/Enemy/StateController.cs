using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Titan.Character.Enemy.FSM;
using Titan.Battle;

namespace Titan.Character.Enemy
{
    // @Memo
    // 만약 성능상의 문제가 생길 경우
    // 주요 변수를 cache해서 해결할 것
    // Cache 대상
    // - blocked sight : ray cast가 상대적으로 가벼워도 자주 사용하기에는 비싸다.
    // - distance between target : 상대적으로 계산이 수월한 변수. cache 계산하기도 편하고 이것도 고려해 볼 것.
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateController : MonoBehaviour
    {
        // @Refactor
        // 추후에 Enemy를 Data화할 것
        [Header("Enemy Data")]
        [SerializeField] private bool _isBoss = false;
        public bool IsBoss => _isBoss;
        public float MaxHealth = 3000f;// Stat System으로 들어갈 것
        public float CurHealth = 3000f; // Stat System으로 들어갈 것


        [Header("Behaviour Data")]
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
        // 현재 관심 지점
        // Action에 따라 작동 방법이 다르다.
        // Focus가 켜질 경우 Personal Target을 바라본다. <- Look, Focus Decision에서 Update
        // Aim이 켜질 경우 Personal Target을 조준한다.
        // 특정 이동의 경우 Personal Target으로 이동을 한다.
        [HideInInspector] public Vector3 PersonalTarget = Vector3.zero;
        [HideInInspector] public bool TargetInSight; // Target을 발견했는가
        [HideInInspector] public bool IsFocusTarget = false; // 전투 상태 중에서 목표를 주시하는가. True가 되면 personal target을 바라본다. personal target은 각각의 decision에서 설정해 준다.
        [HideInInspector] public bool IsStraffing = false; // Straff 활성화 여부
        [HideInInspector] public bool IsAimTarget = false; // 전투 상태 중에서 조준 동작 실행 여부
        [HideInInspector] public bool IsAligned;
        [HideInInspector] public bool IsAttack;
        public bool IsInvincible = false;
        
        [Header("FSM Settings")]
        [Tooltip("공격하기 위해 접근하는 거리")]
        public float AttackRange = 1.5f;
        [Tooltip("공격하고 나서 플레이어와 유지하는 거리")]
        public float CombatSpacing = 4f;
        [Tooltip("공격 유지 거리, 이 거리를 벗어나면 접근을 시도한다")]
        public float RepositionThreshold = 7f; // Will move to attack component

        // Cache and access from state
        [HideInInspector] public NavMeshAgent Nav;
        [HideInInspector] public EnemyAnimation EnemyAnim;
        [HideInInspector] public EnemyAttackController EnemyAttackController;
        [HideInInspector] public EnemyHealth EnemyHealth;
        [HideInInspector] public EnemyVariables Variables;
        private MinimapMarker _marker;
        
#if UNITY_EDITOR
        public bool DebugMode = false;
#endif

        private void Awake()
        {
            aiActive = true;

            Nav = GetComponent<NavMeshAgent>();
            EnemyAnim = gameObject.AddComponent<EnemyAnimation>();
            EnemyAttackController = EnemyAnim.Animator.gameObject.AddComponent<EnemyAttackController>();
            EnemyHealth = GetComponent<EnemyHealth>();
            _marker = GetComponent<MinimapMarker>();

            Debug.Assert(currentState, "State is not set");
            Debug.Assert(remainState, "State is not set");

            if(!currentState || !remainState)
            {
                aiActive = false;
            }

            PatrolWaypoints = PatrolWaypoints.Where(x => x != null).ToArray();
        }

        private void Start()
        {
            currentState.OnEnableActions(this);

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

        // 만약 사용 빈도가 높을 경우 Cache 사용할 것
        public float GetPersonalTargetDist()
        {   
            return TargetInSight ? Vector3.Distance(transform.position, PersonalTarget) : 0f;
        }

        public void OnHitHandler()
        {
            // 경직이 가능하다면, 가령 유닛마다 경직 저항치가 있을 수 있지
            
        }

        public void OnDeathHandler()
        {
            aiActive = false;
            if(TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }
            Nav.enabled = false;
            _marker.MarkerOn = false;

            EnemyAnim.Animator.SetTrigger(AnimatorKey.Enemy.DeathTrigger);
            // Death Sound
            // Death Vfx
            EventBus.RaiseEvent<EnemyDeadEvent>(new EnemyDeadEvent
            {
                EnemyID = gameObject.name,
                IsBoss = IsBoss
            });

            Destroy(gameObject, GeneralStats.DeathDelayTime);
        }
    }
}

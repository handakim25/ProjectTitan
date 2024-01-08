using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    // @Refactor
    // Animation을 관리하는 PlayerAnim을 고려할 것
    // OnGroundEnter, OnGroundExit을 GroundChecker로 옮길 것
    // 되도록 Behavioiur를 관리하도록 분리한다. 가령, 그라운드 로직이 수정되면 이 코드가 수정될 이유가 없다.

    /// <summary>
    /// Behaviour를 등록해서 관리하는 클래스
    /// 각각의 Behaviour는 Behaviour Code를 통해서 선택된다.
    /// Current Behaviour는 일반적으로 실행되는 행동이다.
    /// Register 동작을 통해서 동작을 변경할 수 있다.
    /// Override는 오버라이드 동작이다.
    /// Current Behaviour를 멈추고 Override 동작들을 호출한다.
    /// Override는 복수의 동작들이 가능하다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class BehaviourController : MonoBehaviour
    {
        #region Variables
        
        // 일반 동작들
        private List<GenericBehaviour> _behaviours;
        // 오버라이드 동작들 우선 동작한다.
        private List<GenericBehaviour> _overrideBehaviours;

        /// <summary>
        /// 현재 동작하는 행동 코드, RegisterBehaviour를 통해서 변경할 수 있다.
        /// </summary>
        private int _currentBehaviourCode;
        /// <summary>
        /// 기본 동작 행동 코드, 기본 동작은 행동이 비어 있을 경우 호출된다.
        /// </summary>
        private int _defaultBehaviourCode;
        /// <summary>
        /// 잠겨 있는 동작 코드, 잠겨 있을 경우 Update되지 않는다.
        /// 잠겨있지 않을 경우 0이다
        /// </summary>
        private int _behaviourLocked;

        // Cahching
        // 각각의 Bahaviour에서 접근할 수 있도록 한다.
        public Animator Animator {get; private set;}
        public CharacterController CharacterController {get; private set;}
        public GroundChecker GroundChecker {get; private set;}
        public Camera Camera {get; private set;}
        public PlayerInput PlayerInput {get; private set;}
        public PlayerMove PlayerMove {get; private set;}
        public PlayerController Controller {get; private set;}
        public PlayerStatus Status => Controller.Status;

        // Animator Layer Index
        // 공격 애니메이션을 처리하기 위한 Layer Index
        private int BasicAnimatorLayerIndex = 0;
        private int SkillAnimatorLayerIndex = 0;
        private int HyperAnimatorLayerIndex = 0;

        /// <summary>
        /// 현재 프레임에서의 그라운드에 있는 지의 여부
        /// </summary>
        /// <value></value>
        [field : SerializeField] public bool IsGround {get; private set;}
        /// <summary>
        /// 지면에 닿았을 때 호출되는 이벤트
        /// </summary>
        public event System.Action OnGroundEnter;
        /// <summary>
        /// 지면에서 벗어났을 때 호출되는 이벤트
        /// </summary>
        public event System.Action OnGroundExit;
        private bool _applyGravity;
        public bool ApplyGravity {
            get
            {
                return _applyGravity;
            }
            set
            {
                _applyGravity = value;
                PlayerMove.IsApplyGravity = _applyGravity;
            }
        }

        private Vector3 _lastDirection = Vector3.zero;

#if UNITY_EDITOR
        public bool DebugMode = false;
#endif

        // Ground Check

        #endregion Variables

        #region Unity Methods
        
        private void Awake()
        {
            _behaviours = new List<GenericBehaviour>();
            _overrideBehaviours = new List<GenericBehaviour>();

            // Caching
            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
            GroundChecker = GetComponent<GroundChecker>();
            Camera = Camera.main;
            PlayerInput = GetComponent<PlayerInput>();
            PlayerMove = GetComponent<PlayerMove>();
            Controller = GetComponent<PlayerController>();

            // Get Animator Index
            BasicAnimatorLayerIndex = Animator.GetLayerIndex("Basic");
            SkillAnimatorLayerIndex = Animator.GetLayerIndex("Skill");
            HyperAnimatorLayerIndex = Animator.GetLayerIndex("Hyper");
        }
        
        private void Start()
        {
            IsGround = GroundChecker.IsGround();
        }

        private void Update()
        {
            // Ground State 정리
            // 1. Animtion : Fall Animation에 사용
            // 2. Fall 상태가 되면 Jump 불가능, Move 불가능
            UpdatGroundState();

            // 등록되지 않은 함수들은 Update되지 않는다.

            // 잠겨 있거나 오버라이드가 되지 않았을 경우
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => 
                    behaviour.isActiveAndEnabled && 
                    behaviour.BehaviourCode == _currentBehaviourCode);

                if(curBehaviour != null)
                {
                    curBehaviour.LocalUpdate();
                }
            }
            // 오버라이드가 되어 있을 경우
            else
            {
                foreach(GenericBehaviour behaviour in _overrideBehaviours)
                {
                    behaviour.LocalUpdate();
                }
            }

            PlayerMove.Move();
            Animator.SetBool(AnimatorKey.Player.IsGround, IsGround);
            Animator.SetBool(AnimatorKey.Player.HasMoveInput, PlayerInput.MoveDir != Vector2.zero);
            Animator.SetFloat(AnimatorKey.Player.BasicStateTime, Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(1).normalizedTime, 1f));
        }

        private void FixedUpdate()
        {
            bool isAnyBehaviourUpdate = false;
            // 잠겨 있거나 오버라이드가 되지 않았을 경우
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled && behaviour.BehaviourCode == _currentBehaviourCode);
                if(curBehaviour != null)
                {
                    curBehaviour.LocalFixedUpdate();
                    isAnyBehaviourUpdate = true;
                }
            }
            // 오버라이드가 되어 있을 경우
            else
            {
                foreach(GenericBehaviour behaviour in _overrideBehaviours)
                {
                    behaviour.LocalFixedUpdate();
                }
            }

            // Overide된 동작이 없고 Lock이 되어 있을 경우
            if(!isAnyBehaviourUpdate && _overrideBehaviours.Count == 0)
            {
            }
        }

        private void LateUpdate()
        {
            // 잠겨 있거나 오버라이드가 되지 않았을 경우
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled && behaviour.BehaviourCode == _currentBehaviourCode);
                if(curBehaviour != null)
                {
                    curBehaviour.LocalLateUpdate();
                }
            }
            // 오버라이드가 되어 있을 경우
            else
            {
                foreach(GenericBehaviour behaviour in _overrideBehaviours)
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        
        #endregion Unity Methods

        #region Conroller Methods
        
        /// <summary>
        /// 행동을 구독한다. 구독된 행동들은 관리가 된다.
        /// </summary>
        /// <param name="behaviour">등록을 하고자 하는 행동</param>
        public void SubscribeGenericBehaviour(GenericBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        /// <summary>
        /// 디폴트 행동을 설정한다. 디폴트 행동은 행동이 비어 있을 경우 호출된다.
        /// </summary>
        /// <param name="behaivourCode">행동 코드</param>
        public void RegisterDefaultBehaviour(int behaivourCode)
        {
            _defaultBehaviourCode = behaivourCode;
            _currentBehaviourCode = behaivourCode;

            ExecuteFunctionWithBehaviour(behaivourCode, (behaviour) => behaviour.OnEnter());
        }

        /// <summary>
        /// 일반 동작으로 등록한다. 등록을 하기 위해서는 현재 행동이 없어야 한다.
        /// 등록을 하면 다른 행동은 등록을 할 수 없다. 이 경우는 다른 동작을 하지 않는다.
        /// 행동을 해제하기 위해서는 UnregisterBehaviour를 호출
        /// </summary>
        /// <param name="behaviourCode">등록하기 위한 행동의 코드</param>
        public void RegisterBehaviour(int behaviourCode)
        {
            if(SearchBehaviour(behaviourCode) == null)
            {
#if UNITY_EDITOR
                if(DebugMode)
                {
                    Debug.Log($"Not registered Code");
                }
#endif
                return;
            }            
            // 현재 행동이 비어있을 경우, 즉 기본 행동일 때만 새로운 행동을 등록할 수 있다.
            if(_currentBehaviourCode == _defaultBehaviourCode)
            {
#if UNITY_EDITOR
                if(DebugMode)
                {
                    Debug.Log($"{GetCurrentBehaviour().GetType()} : Exit");
                }
#endif
                ExecuteFunctionWithBehaviour(_currentBehaviourCode, (behaviour) => behaviour.OnExit());
                _currentBehaviourCode = behaviourCode;
#if UNITY_EDITOR
                if(DebugMode)
                {
                    Debug.Log($"{GetCurrentBehaviour().GetType()} : Enter");
                }                
#endif
                ExecuteFunctionWithBehaviour(_currentBehaviourCode, (behaviour) => behaviour.OnEnter());
            }
        }

        /// <summary>
        /// 동작을 등록 해제한다. 등록 해제는 현재 등록한 상태만 해제할 수 있다.
        /// 등록을 해제하면 기본 상태로 돌아온다.
        /// </summary>
        /// <param name="behaviourCode">등록하기 위한 행동의 코드</param>
        public void UnregisterBehaviour(int behaviourCode)
        {
            if(_currentBehaviourCode == behaviourCode)
            {
#if UNITY_EDITOR
                if(DebugMode)
                {
                    Debug.Log($"{GetCurrentBehaviour().GetType()} : Exit");
                }
#endif                
                ExecuteFunctionWithBehaviour(_currentBehaviourCode, (behaviour) => behaviour.OnExit());
                _currentBehaviourCode = _defaultBehaviourCode;
#if UNITY_EDITOR
                if(DebugMode)
                {
                    Debug.Log($"{GetCurrentBehaviour().GetType()} : Enter");
                }
#endif                
                ExecuteFunctionWithBehaviour(_currentBehaviourCode, (behaviour) => behaviour.OnEnter());
            }
        }

        /// <summary>
        /// 행동을 Override 한다. 현재 행동을 중지하고 Override를 호출한다.
        /// Override는 여러개 가능하다.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool OverrideWithBehaviour(GenericBehaviour behaviour)
        {
            if(_overrideBehaviours.Contains(behaviour))
            {
                return false;
            }
            if(_overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled &&behaviour.BehaviourCode == _currentBehaviourCode);
                if(curBehaviour != null)
                {
                    curBehaviour.OnOverride();
                }
            }

            _overrideBehaviours.Add(behaviour);
            return true;
        }

        /// <summary>
        /// 행동을 Override 해제한다. Override가 되어 있지 않으면 아무런 동작을 하지 않는다.
        /// </summary>
        /// <param name="behaviour">Override를 해제할 행동</param>
        /// <returns>해제했으면 True, 없을 경우 False</returns>
        public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
        {
            if(_overrideBehaviours.Contains(behaviour))
            {
                _overrideBehaviours.Remove(behaviour);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Behaviour가 Overide 되어 있는지 확인하는 함수
        /// null을 집어 넣으면 Override 되어 있는지를 확인한다.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool IsOverriding(GenericBehaviour behaviour = null)
        {
            if(behaviour == null)
            {
                return _overrideBehaviours.Count() > 0;
            }
            return _overrideBehaviours.Contains(behaviour);
        }

        public bool IsCurrentBehaviour(int behaivourCode) => _currentBehaviourCode == behaivourCode;
        
        /// <summary>
        /// Behaviour Code가 잠겨있는지 확인한다. behaviourCode가 0일 경우 잠겨 있는지만 확인한다.
        /// </summary>
        /// <param name="behaviourCode">잠그기 전에 잠글 수 있는 상태를 확인할 수 있다.</param>
        /// <returns></returns>
        public bool GetTempLockStatus(int behaviourCode = 0)
        {
            // _behaviourLocked != 0 : 잠겨 있을 경우 true, 아닐 경우 false
            // _behaviourLocked != behaviourCode : 잠겨 있는 행동과 같을 경우 false, 다를 경우 true

            // _behaviourCode = 0일 경우
            // _behaviourLocked != 0 && _behaviourLocked != 0
            // 앞뒤가 같은 코드가 된다.
            // 즉, 잠겨 있는지만 확인하는 코드이다.

            // _behaviourCode != 0일 경우
            // a. _behaviourLocked != 0 : true
            // b. _behaviourLocked == behaviourCode : 자기 자신이 Lock을 걸은 상태라면, false
            // c. _behaviourLocked != behaviourCode : 자기 자신이 Lock을 걸은 것이 아니라면 true
            // 즉, Lock을 확인하는데 자기 자신이 걸은 것이라면 false, 다른 것이 걸려 있으면 true
            // 해제하기 전에 미리 체크할 수 있다.
            return _behaviourLocked != 0 && _behaviourLocked != behaviourCode;
        }

        /// <summary>
        /// Behaviour를 잠근다. 현재 상태가 잠겨있지 않아야 한다.
        /// </summary>
        /// <param name="behaivourCode"></param>
        public void LockTempBehaviour(int behaivourCode)
        {
            if(_behaviourLocked == 0)
            {
                _behaviourLocked = behaivourCode;
            }
        }

        /// <summary>
        /// 잠긴 Behaviour를 해제한다. 현재 상태가 잠겨있어야 한다.
        /// </summary>
        /// <param name="behaviourCode"></param>
        public void UnLockTempBehaviour(int behaviourCode)
        {
            if(_behaviourLocked == behaviourCode)
            {
                _behaviourLocked = 0;
            }
        }

        /// <summary>
        /// 현재 행동을 반환한다.
        /// </summary>
        /// <returns>찾지 못했을 경우 null을 반환</returns>
        public GenericBehaviour GetCurrentBehaviour() => SearchBehaviour(_currentBehaviourCode);

        /// <summary>
        /// 행동 코드를 통해서 Behaviour를 찾는다.
        /// </summary>
        /// <param name="behaviourCode">찾고자 하는 행동 코드</param>
        /// <returns>행동 코드에 해당하는 행동, 없을 경우 null을 반환</returns>
        private GenericBehaviour SearchBehaviour(int behaviourCode)
        {
            return _behaviours.FirstOrDefault((behaviour) =>
                behaviour.isActiveAndEnabled && 
                behaviour.BehaviourCode == behaviourCode);
        }

        /// <summary>
        /// Behaviour Code를 통해서 Behaviour를 찾고, action을 실행한다.
        /// </summary>
        /// <param name="behaviourCode">찾으려는 행동 코드</param>
        /// <param name="action">Behaviour Delegate</param>
        /// <returns>함수 호출을 성공하면 True, 실패하면 False</returns>
        private bool ExecuteFunctionWithBehaviour(int behaviourCode, System.Action<GenericBehaviour> action)
        {
            var behaviour = SearchBehaviour(behaviourCode);
            if(behaviour != null)
            {
                action(behaviour);
                return true;
            }
            return false;
        }

        #endregion Conroller Methods

        #region Update Controller
        
        // @Note
        // CharacterController는 이전 프레임의 Ground 상태인 것에 주의

        // @Note
        // Update Ground State에서 State를 Update하고
        // 플레이어 로직에서 이를 기반으로 처리(Condition, Apply Gravity, Move)
        // 만약에 로직을 처리하고 다시 Update Ground State로 들어오게 된다면
        // 상황에 따라 Ground Exit / Ground Enter 등을 호출한다
        void UpdatGroundState()
        {
            bool prevGround = IsGround;
            bool curGround = CharacterController.isGrounded;
            // Fall or Jump
            if(!curGround && prevGround)
            {
#if UNITY_EDITOR
                    if(DebugMode)
                    {
                        Debug.Log($"Ground Exit");
                    }
#endif
                OnGroundExit?.Invoke();
            }
            // Land
            else if(curGround && !prevGround)
            {
#if UNITY_EDITOR
                if (DebugMode)
                {
                    Debug.Log($"Ground Enter");
                }
#endif
                OnGroundEnter?.Invoke();
            }

            IsGround = curGround;
        }
        
        #endregion Update Controller

        #region Common Logics
        
        // @Refactor
        // Draw ray in editor code
        /// <summary>
        /// 카메라를 기준으로 이동 방향을 계산한다.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraRelativeMovement()
        {
            Transform cameraTr = Camera.transform;

            // World 기준에서의 Camera의 Forward, Right를 계산
            Vector3 cameraForward = new(cameraTr.forward.x, 0, cameraTr.forward.z);
            Vector3 cameraRigth = new(cameraTr.right.x, 0, cameraTr.right.z);

            // @To-Do Editor 코드로 이동을 고려
            // Camera Forward, Right를 그리기
            Debug.DrawRay(transform.position, cameraForward, Color.blue);
            Debug.DrawRay(transform.position, cameraRigth, Color.green);

            // MoveDir.x : ad Input, 좌우 이동
            // MoveDir.y : ws Input, 앞뒤 이동
            // PlayerInput에서 Normalize된 상태로 넘어오기 때문에 대각성 이동은 문제 없다.
            return cameraForward.normalized * PlayerInput.MoveDir.y + cameraRigth.normalized * PlayerInput.MoveDir.x;
        }

        /// <summary>
        /// targetDir을 향해 회전한다.
        /// </summary>
        /// <param name="targetDir">향하고자 하는 방향</param>
        /// <param name="isImmedate">true일 경우 즉시 바라본다.</param>
        /// <param name="smoothingDamp">Damp 보간값</param>
        public void FaceDirection(Vector3 targetDir, bool isImmedate = false, float smoothingDamp = 20f)
        {
            if(targetDir == Vector3.zero)
            {
                return;
            }

            if(!isImmedate)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), smoothingDamp * Time.deltaTime);            
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(targetDir);
            }
        }                

        /// <summary>
        /// 이동 방향의 마지막을 기록해 둔다. Vector3.zero는 기록되지 않는다.
        /// </summary>
        /// <param name="dir">이동 방향</param>
        public void SetLastDirection(Vector3 dir)
        {
            if(dir == Vector3.zero)
            {
                return;
            }
            _lastDirection = dir;
        }        

        public Vector3 GetLastDirection() => _lastDirection;
        
        #endregion Common Logics
    }
}

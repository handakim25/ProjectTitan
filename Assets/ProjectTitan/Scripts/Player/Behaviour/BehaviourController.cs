using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
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
        /// 보통 동작들이다.
        /// CurrentBehavioiur는 현재 동작이다.
        /// DefaultBehaviour는 기본 동작이다. 동작이 선택되지 않으면 여기로 온다.
        /// behaviourLocked 동작 잠금이다. 일시적으로 동작을 잠가서 동작하지 않게 한다.
        /// </summary>
        private int _currentBehaviour;
        private int _defaultBehaviour;
        private int _behaviourLocked;

        /// <summary>
        /// Cahching 변수들
        /// 동작에서 여기에 접근해서 사용한다.
        /// </summary>
        public Animator Animator {get; private set;}
        public CharacterController CharacterController {get; private set;}
        public GroundChecker GroundChecker {get; private set;}
        public Camera Camera {get; private set;}
        public PlayerInput PlayerInput {get; private set;}
        public PlayerMove PlayerMove {get; private set;}
        public PlayerController Controller {get; private set;}

        /// <summary>
        /// 현재 프레임에서의 그라운드에 있는 지의 여부
        /// </summary>
        /// <value></value>
        [field : SerializeField] public bool IsGround {get; private set;}
        public bool ShowIsGround;
        public event System.Action OnGroundEnter;
        public event System.Action OnGroundExit;

        // Ground Check

        #endregion Variables

        #region Unity Methods
        
        private void Awake()
        {
            _behaviours = new List<GenericBehaviour>();
            _overrideBehaviours = new List<GenericBehaviour>();

            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
            GroundChecker = GetComponent<GroundChecker>();
            Camera = Camera.main;
            PlayerInput = GetComponent<PlayerInput>();
            PlayerMove = GetComponent<PlayerMove>();
            Controller = GetComponent<PlayerController>();
        }
        
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            IsGround = GroundChecker.IsGround();
        }

        private void Update()
        {
            UpdatGroundState();

            // 주의해야할 점은 등록된 behaviour들 중에서만 호출 되는 것이다.
            // 여기에 등록되지 않고 독자적으로 작동하는 것은 여기에 호출되지 않는다.
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled && behaviour.BehavriouCode == _currentBehaviour);
                if(curBehaviour != null)
                {
                    curBehaviour.LocalUpdate();
                }
            }
            else
            {
                foreach(GenericBehaviour behaviour in _overrideBehaviours)
                {
                    behaviour.LocalUpdate();
                }
            }

            PlayerMove.Move();
            Animator.SetBool(AnimatorKey.Player.IsGround, CharacterController.isGrounded);
        }

        private void FixedUpdate()
        {
            bool isAnyBehaviourUpdate = false;
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled && behaviour.BehavriouCode == _currentBehaviour);
                if(curBehaviour != null)
                {
                    curBehaviour.LocalFixedUpdate();
                    isAnyBehaviourUpdate = true;
                }
            }
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
            // 잠겨 있지 않고 오버라이드 되지 않았을 경우
            // 현재 행동만 update
            // 중간에 잠갔을 경우 else로 가지만 Override가 되지 않았으므로 
            // 그냥 통과
            // 잠긴 상태에서 넘어갔을 경우 어찌되든 자기 자신이 풀어주어야 한다.
            if(_behaviourLocked > 0 || _overrideBehaviours.Count == 0)
            {
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled && behaviour.BehavriouCode == _currentBehaviour);
                curBehaviour?.LocalLateUpdate();
            }
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
        
        public void SubscribeGenericBehaviour(GenericBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public void RegisterDefaultBehaviour(int behaivourCode)
        {
            _defaultBehaviour = behaivourCode;
            _currentBehaviour = behaivourCode;
        }

        /// <summary>
        /// 일반 동작으로 등록한다. 등록을 하기 위해서는 행동이 비어 있어야 한다.
        /// 등록을 하면 다른 행동은 등록을 할 수 없다.
        /// </summary>
        /// <param name="behaviourCode">등록하기 위한 행동의 코드</param>
        public void RegisterBehaviour(int behaviourCode)
        {
            if(_currentBehaviour == _defaultBehaviour)
            {
                _currentBehaviour = behaviourCode;
            }
        }

        /// <summary>
        /// 동작을 등록 해제한다. 등록 해제는 현재 등록한 상태만 해제할 수 있다.
        /// 등록을 해제하면 기본 상태로 돌아온다.
        /// </summary>
        /// <param name="behaivourCode">등록하기 위한 행동의 코드</param>
        public void UnregisterBehaviour(int behaivourCode)
        {
            if(_currentBehaviour == behaivourCode)
            {
                _currentBehaviour = _defaultBehaviour;
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
                var curBehaviour = _behaviours.FirstOrDefault((behaviour) => behaviour.isActiveAndEnabled &&behaviour.BehavriouCode == _currentBehaviour);
                curBehaviour?.OnOverride();
            }

            _overrideBehaviours.Add(behaviour);
            return true;
        }

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

        public bool IsCurrentBehaviour(int behaivourCode) => _currentBehaviour == behaivourCode;
        /// <summary>
        /// Lock 상황을 확인한다.
        /// 
        /// </summary>
        /// <param name="behaviourCode"></param>
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
            return (_behaviourLocked != 0 && _behaviourLocked != behaviourCode);
        }

        public void LockTempBehaviour(int behaivourCode)
        {
            if(_behaviourLocked == 0)
            {
                _behaviourLocked = behaivourCode;
            }
        }

        public void UnLockTempBehaviour(int behaviourCode)
        {
            if(_behaviourLocked == behaviourCode)
            {
                _behaviourLocked = 0;
            }
        }
        
        #endregion Conroller Methods

        #region Common Logics
        
        void UpdatGroundState()
        {
            bool curGround = CharacterController.isGrounded;
            if(!curGround && IsGround)
            {
                OnGroundExit?.Invoke();
            }
            else if(curGround && !IsGround)
            {
                OnGroundEnter?.Invoke();
            }

            IsGround = curGround;
        }
        
        #endregion Common Logics
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan.Character.Player
{
    public abstract class AttackBehaviour : GenericBehaviour
    {
        // @Refactor
        // Skill Data가 전체적으로 묶여서 저장이 되어야 될 것 같다.
        // 추후에 수정할 것
        [Header("Skill Data")]
        [SerializeField] private string _attackName = "AttackDefaultName";
        [SerializeField] private Sprite _iconSprite = null;
        [SerializeField] private AttackType _attackType = AttackType.Basic;
        [SerializeField] private float _coolTime = 10f;
        [Range(0, 1)]
        [Tooltip("0 : 초기 쿨타임 없음 / 1 : 처음부터 쿨타임 다 차 있음")]
        [SerializeField] private float _initialCoolTime = 0f;
        [SerializeField] private int _requireEnergy = 0;
        [SerializeField] protected List<AttackData> _attackList = new List<AttackData>();
        // @Refactor
        // Attack index is not used. Remove from code
        protected int curAttackIndex = 0;
        [SerializeField] protected int _animationIndex = 0;
        [SerializeField] private LayerMask _targetMask;

        /// <summary>
        /// 현재 Cooltime, [0, _coolTime], 0에서 _coolTime까지 올라간다.
        /// </summary>
        private float _curCoolTime;
        public bool CanFire => _curCoolTime >= _coolTime;

        #region Unity Methods
        
        private void Start()
        {
            _controller.SubscribeGenericBehaviour(this);

            var animatorBehaviours = _controller.Animator.GetBehaviours<AttackStateMachineBehaviour>();
            var animatorBehaviour = animatorBehaviours.FirstOrDefault((behaviour) => behaviour.AttackType == _attackType);
            if(animatorBehaviour == null)
            {
                Debug.LogError($"Attack State Machine is missing");
            }
            else
            {
                animatorBehaviour.OnAttackEnd += AttackEndHandler;
            }

            switch(_attackType)
            {
                case AttackType.Basic:
                    _controller.PlayerInput.OnBasicPerformed += AttackPerformedHandler;
                    break;
                case AttackType.Skill:
                    _controller.PlayerInput.OnSkillPerformed += AttackPerformedHandler;
                    break;
                case AttackType.Hyper:
                    _controller.PlayerInput.OnHyperPerformed += AttackPerformedHandler;
                    break;
            }

            _curCoolTime = _coolTime * _initialCoolTime;     
            InitStatus();
        }

        private void Update()
        {
            // Update Cool Time
            if(_curCoolTime < _coolTime)
            {
                _curCoolTime += Time.deltaTime;
                UpdateStatus();
            }
        }
        
        #endregion Unity Methods

        private void InitStatus()
        {
            switch(_attackType)
            {
                case AttackType.Basic:
                    _controller.Status.BasicIcon = _iconSprite;
                    _controller.Status.BasicCooltime = _coolTime;
                    break;
                case AttackType.Skill:
                    _controller.Status.SkillIcon = _iconSprite;
                    _controller.Status.SkillCooltime = _coolTime;
                    break;
                case AttackType.Hyper:
                    _controller.Status.HyperIcon = _iconSprite;
                    _controller.Status.HyperCooltime = _coolTime;
                    break;
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            switch(_attackType)
            {
                case AttackType.Basic:
                    _controller.Status.BasicCurCooltime = _curCoolTime;
                    break;
                case AttackType.Skill:
                    _controller.Status.SkillCurCooltime = _curCoolTime;
                    break;
                case AttackType.Hyper:
                    _controller.Status.HyperCurCooltime = _curCoolTime;
                    break;
            }
        }

        // 진입점은 AttackPerformHandler
        // Attack Perform Handler에서 애니메이션 발생 여부를 선택
        // OnEnter로 들어가면 Animation 제어
        // Pre condition : Animatino State가 None 상태로 있어야 한다.
        public override void OnEnter()
        {
            FaceTarget();
            _curCoolTime = 0f;
        }

        #region Callbacks
        
        // Charge, Range Attack이랑 구별 지어야 한다
        // 하지만 그러면 상속으로 처리하면 되지 않을까?
        protected virtual void AttackPerformedHandler()
        {
            if(!CanAttack())
            {
                return;
            }

            _controller.RegisterBehaviour(BehaviourCode);
        }

        // Attack 애니메이션이 종료되었을 때 호출되는 함수
        protected virtual void AttackEndHandler()
        {
            _controller.UnregisterBehaviour(BehaviourCode);
        }

        // Animation Event Callback
        // Called attack impact time
        // @To-DO
        // 상속을 통해서 해결할 필요가 없다.
        // 인자를 주고 받고 처리하면 될 것 같은데
        public void ExecuteAttack()
        {
            if(_controller.IsCurrentBehaviour(BehaviourCode))
            {
                PerformAttack();
            }
        }        
        
        #endregion Callbacks

        protected virtual void PerformAttack()
        {
            Debug.Log($"Perform Attack is not overided.");
        }

        #region Utility Methods
        
        /// <summary>
        /// Return Animator index by Object's Attack type
        /// </summary>
        /// <returns></returns>
        protected int GetAnimIndexParam()
        {
            return _attackType switch
            {
                AttackType.Basic => AnimatorKey.Player.BasicIndex,
                AttackType.Skill => AnimatorKey.Player.SkillIndex,
                _ => AnimatorKey.Player.BasicIndex,
            };
        }

        protected int GetAnimTriggerParam()
        {
            return _attackType switch
            {
                AttackType.Basic => AnimatorKey.Player.BasicTrigger,
                AttackType.Skill => AnimatorKey.Player.SkillTrigger,
                _ => AnimatorKey.Player.SkillTrigger,
            };
        }

        protected bool CanAttack()
        {
            return _controller.IsGround && CanFire;
        }

        public void FaceTarget()
        {
            var nearestGo = FindTarget();
            if(nearestGo == null)
            {
                return;
            }

            var faceDir = nearestGo.transform.position - transform.position;
            faceDir.y = 0f;
            _controller.FaceDirection(faceDir, true);
        }

        private GameObject FindTarget()
        {
            var colliders = Physics.OverlapSphere(transform.position, 3f, _targetMask);
            GameObject minGo = null;
            float min = float.PositiveInfinity;
            foreach(Collider curCollider in colliders)
            {
                float curDist = Vector3.Distance(transform.position, curCollider.transform.position);
                if(curDist < min)
                {
                    min = curDist;
                    minGo = curCollider.gameObject;
                }
            }

            return minGo;
        }
        
        #endregion Utility Methods
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan.Character.Player
{
    public abstract class AttackBehaviour : GenericBehaviour
    {
        [Header("Skill Data")]
        [SerializeField] private AttackType _attackType = AttackType.Basic;
        [SerializeField] public float _coolTime = 10f;
        [Range(0, 1)]
        [Tooltip("0 : 초기 쿨타임 없음 / 1 : 처음부터 쿨타임 다 차 있음")]
        [SerializeField] public float _initialCoolTime = 0f;
        [SerializeField] public int _requireEnergy = 0;
        [SerializeField] protected List<AttackData> _attackList = new List<AttackData>();
        // @Refactor
        // Attack index is not used. Remove from code
        protected int curAttackIndex = 0;
        [SerializeField] protected int _animationIndex = 0;
        [SerializeField] private LayerMask _targetMask;

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
        }

        private void Update()
        {
            // Update Cool Time
            if(_curCoolTime < _coolTime)
            {
                _curCoolTime += Time.deltaTime;
            }
        }
        
        #endregion Unity Methods

        // 진입점은 AttackPerformHandler
        // Attack Perform Handler에서 애니메이션 발생 여부를 선택
        // OnEnter로 들어가면 Animation 제어
        // Pre condition : Animatino State가 None 상태로 있어야 한다.
        public override void OnEnter()
        {
            FaceTarget();
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
            Debug.Log($"Nearest Go : {nearestGo}");
            if(nearestGo == null)
            {
                return;
            }

            var faceDir = nearestGo.transform.position - transform.position;
            faceDir.y = 0f;
            Debug.Log($"Face Dir : {faceDir}");
            _controller.FaceDirection(faceDir, true);
        }

        private GameObject FindTarget()
        {
            var colliders = Physics.OverlapSphere(transform.position, 3f, _targetMask);
            Debug.Log($"Find {colliders.Length} count");
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

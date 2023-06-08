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
        protected int curAttackIndex = 0;
        [SerializeField] protected int _animationIndex = 0;

        private float _curCoolTime;
        public bool CanFire => _curCoolTime >= _coolTime;

        #region Unity Methods
        
        private void Start()
        {
            _controller.SubscribeGenericBehaviour(this);

            var behaviours = _controller.Animator.GetBehaviours<AttackStateMachineBehaviour>();
            var behaviour = behaviours.FirstOrDefault((behaviour) => behaviour.AttackType == _attackType);
            behaviour.OnAttackEnd += AttackEndHandler;

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

        #region Callbacks
        
        // Charge, Range Attack이랑 구별 지어야 한다
        protected virtual void AttackPerformedHandler()
        {
            if(!_controller.IsGround || !CanFire)
            {
                return;
            }
        }

        protected virtual void AttackEndHandler()
        {
            _controller.UnregisterBehaviour(BehaviourCode);
        }
        
        #endregion Callbacks

        protected void PerformAttack(AttackData attack)
        {

        }

        #region Utility Methods
        
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
        
        #endregion Utility Methods
    }
}

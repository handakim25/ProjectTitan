using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

namespace Titan.Character.Player
{
    public abstract class AttackBehaviour : GenericBehaviour
    {
        [Header("Skill Data")]
        [SerializeField] private SkillType _skillType = SkillType.Basic;
        [SerializeField] public float _coolTime = 10f;
        [Range(0, 1)]
        [Tooltip("0 : 초기 쿨타임 없음 / 1 : 처음부터 쿨타임 다 차 있음")]
        [SerializeField] public float _initialCoolTime = 0f;
        [SerializeField] public int _requireEnergy = 0;

        private float _curCoolTime;
        public bool CanFire => _curCoolTime >= _coolTime;

        [SerializeField] private int _attackIndex = 0;

        private void Start()
        {
            Debug.Log($"Attack Start");
            switch(_skillType)
            {
                case SkillType.Skill:
                    _controller.PlayerInput.OnSkillPerformed += AttackPerformedHandler;
                    break;
                case SkillType.Hyper:
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

        // Charge, Range Attack이랑 구별 지어야 한다
        protected virtual void AttackPerformedHandler()
        {
            if(!_controller.IsGround || !CanFire)
            {
                return;
            }

            _controller.RegisterBehaviour(BehaviourCode);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @Refactor
// Skill 관련을 묶어서 관리하는 클래스 필요
// 해당 Class를 통해서 SKill Icon Controller와 관리할 것
// 현재 시스템 상에서는 복잡한 Skill 구현을 할 때, Player Status, Skill Icon Controller, Skill Pannel Controller 등 수정되어야 할 곳이 많다.

namespace Titan.Character.Player
{
    public class PlayerStatus
    {
        private bool _isDirty = false;
        private float _health;
        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                _isDirty = true;
            }
        }

        private float _maxHealth;
        public float MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                _isDirty = true;
            }
        }

        // 현재 기준으로 궁극기만 Energy를 가지고 있다.
        private float _maxEnergy;
        public float MaxEnergy
        {
            get => _maxEnergy;
            set
            {
                _maxEnergy = value;
                _isDirty = true;
            }
        }

        private float _curEnergy;
        public float CurEnergy
        {
            get => _curEnergy;
            set
            {
                _curEnergy = value;
                _isDirty = true;
            }
        }

        public Sprite BasicIcon;
        private float _basicCooltime;
        public float BasicCooltime
        {
            get => _basicCooltime;
            set
            {
                _basicCooltime = value;
                _isDirty = true;
            }
        }
        private float _basicCurCooltime;
        public float BasicCurCooltime
        {
            get => _basicCurCooltime;
            set
            {
                _basicCurCooltime = value;
                _isDirty = true;
            }
        }

        public Sprite SkillIcon;
        private float _skillCooltime;
        public float SkillCooltime
        {
            get => _skillCooltime;
            set
            {
                _skillCooltime = value;
                _isDirty = true;
            }
        }
        private float _skillCurCooltime;
        public float SkillCurCooltime
        {
            get => _skillCurCooltime;
            set
            {
                _skillCurCooltime = value;
                _isDirty = true;
            }
        }

        public Sprite HyperIcon;
        private float _hyperCooltime;
        public float HyperCooltime
        {
            get => _hyperCooltime;
            set
            {
                _hyperCooltime = value;
                _isDirty = true;
            }
        }
        private float _hyperCurCooltime;
        public float HyperCurCooltime
        {
            get => _hyperCurCooltime;
            set
            {
                _hyperCurCooltime = value;
                _isDirty = true;
            }
        }

        public bool IsDirty => _isDirty;
        /// <summary>
        /// Set Dirty Flag to false
        /// </summary>
        public void ResetDirty() => _isDirty = false;
    }
}

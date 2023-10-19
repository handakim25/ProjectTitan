using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

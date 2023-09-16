using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Character.Enemy;


namespace Titan.Battle
{
    // Controller를 알 수 밖에 없어. 데미지 계산을 해야되잖아.
    // 데미지 계산은 Stat Systme 구축 뒤로 갈 것
    public class EnemyHealth : UnitHealth
    {
        [Header("Damage Font")]
        [SerializeField] private HumanBodyBones damageFontTarget = HumanBodyBones.Chest;
        [SerializeField] private Vector3 _damageFontOffset = new Vector3(0f, 0f, 0f);
        private Transform _damageFontPos;

        [Header("Health Bar UI")]
        [SerializeField] private GameObject _healthBarPrefab;
        [SerializeField] private float _healthUIOffset = 0.5f;
        private HealthUI _healthUI;

        // Cache
        StateController _controller;

        private void Awake()
        {
            var animator = GetComponentInChildren<Animator>();
            _damageFontPos = animator.GetBoneTransform(damageFontTarget);

            _controller = GetComponent<StateController>();

            var healthGo = Instantiate(_healthBarPrefab, transform);
            float height = animator.GetBoneTransform(HumanBodyBones.Head).position.y;
            healthGo.transform.localPosition = new(0, height + _healthUIOffset, 0);

            _healthUI = healthGo.GetComponentInChildren<HealthUI>();
            _healthUI.UpdateHealthUI(_controller.MaxHealth, _controller.CurHealth);
        }

        // Unit health ui
        public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject attacker = null)
        {
            // enemy controller로 넘길 것은 넘겨서 해결
            // enemy controller가 animator를 제어할 것
            // controller가 중심이니까 controller를 중심으로 할 것
            // Damage Font
            // Hit Sound
            // Hit Animation
            // Hit Vfx
            // Update Health bar
            // health bar visible은 여기가 아니라 controller에서 처리할 것
            // 비전투 상황에서는 invisible
            // 전투 상황에서 피격 시에 visible

            // Damage 계산이 필요한 부분?
            // 딜량 계산이 필요
            // 최대 데미지 업적
            // 데미지 생성은 이렇게 다양한 부분이 얽히는 부분이야.
            // 해결하기 위해서는 직접 생성을 하는 것은 해서는 안되.
            // Event Bus를 사용하면 어떨까?

            // 순서를 정해서 구현해야 된다.
            // 1. Event Bus 구현
            // 이유는 monster death 등의 상황에서도 구현을 해야되기 때문이다.
            // 2. Damage Font Manager 구현 -> Instantiate

            // 3. enemy controller 구현
            // 여기서부터는 damage font랑 상관 없는 부분이야.
            // 나누어서 해결, 한 번에 하나

            // 즉, 지금해야 할 점은
            // - Evnet bus 구현
            // - Event bus 연결
            // - Damage Font Manager 구현
            // - Damage Font Manager을 Event bus에 연결
            // - Event가 온다면 Damage Font Manager가 Damage font를 생성
            // Damage Font : prefab
            // - pooling
            // - world space
            // - canvas를 따로 쓸까?
            // Timer 구현

            // Temp code
            // will be replaced with event bus
            if(!isAlive || _controller.IsInvincible)
            {
                return;
            }

            // Do some damage Calculation

            DamageFontManager.Instance.SpawnDamageFont(_damageFontPos.position + _damageFontOffset, damage, false, Color.red);

            // Update Health
            // 이거 불사 같은 거 걸리면 어떻게 핸들링하지?
            _controller.CurHealth -= damage;
            if(_controller.CurHealth <= 0f)
            {
                Dead();
            }
            else
            {
                OnHit?.Invoke();
            }

            _healthUI.UpdateHealthUI(_controller.MaxHealth, _controller.CurHealth);
        }

        public override void Dead()
        {
            base.Dead();

            isAlive = false;
            _healthUI.gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Titan.Character;
using UnityEngine;
using UnityEngine.AI;

namespace Titan.Character.Enemy
{
    /// <summary>
    /// Enemy의 Animation 제어
    /// FSM에 의해 상태 등이 제어가 된다면
    /// Animation에서 해당 동작에 맞는 움직임을 재생
    /// 또한 회전도 담당한다.
    /// </summary>
    public class EnemyAnimation : MonoBehaviour
    {
        private float _angularSpeed;
        
        public float AngularSpeed => _angularSpeed;

        private Animator _animator;
        private StateController _controller;
        private NavMeshAgent _nav;

        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<StateController>();
            _nav = GetComponent<NavMeshAgent>();

            _nav.updateRotation = false;
        }

        private void Update()
        {
            NavAnimUpdate();
            UpdateRotation();
        }

        // Nav Mesh 상황에 맞춰서 Animation Update
        // Memo : desiredVelocity는 Speed가 0일 경우 마찮가지로 0이 된다.
        // 따라서 직접 회전을 시켜주어야할 필요가 있다.
        private void NavAnimUpdate()
        {
            float speed = _nav.desiredVelocity.magnitude;
            float angle;
            if(_controller.IsFocusTarget)
            {
                Vector3 target = (_controller.PersonalTarget - transform.position);
                target.y = 0;
                angle = Vector3.SignedAngle(transform.forward, target, transform.up);
                target.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(target);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _controller.GeneralStats.StrafTurnSpeed * Time.deltaTime);
            }

            _animator.SetFloat(AnimatorKey.Enemy.Speed, speed, _controller.GeneralStats.SpeedDampTime, Time.deltaTime);
        }

        // focus mode가 있을 수도 있고
        // focus 상태가 아니면 이동 방향을 바라보면 된다.
        // 어찌되든 angle을 구하고
        // 해당 angle이 되도록 회전을 하면 된다.
        private void UpdateRotation()
        {
            if(_nav.desiredVelocity == Vector3.zero)
            {
                return;
            }
            Quaternion targetRot = Quaternion.LookRotation(_nav.desiredVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 20f * Time.deltaTime);
        }   
    }
}

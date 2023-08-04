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
        private Animator _animator;
        private StateController _controller;
        private NavMeshAgent _nav;

        private void Awake()
        {

            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<StateController>();
            _nav = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            NavAnimUpdate();
        }

        // Nav Mesh 상황에 맞춰서 Animation Update
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
    }
}

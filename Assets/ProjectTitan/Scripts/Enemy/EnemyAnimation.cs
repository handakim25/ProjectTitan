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
        public Vector3 lastDir;

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
        }

        // Nav Mesh 상황에 맞춰서 Animation Update
        // Memo : desiredVelocity는 Speed가 0일 경우 마찮가지로 0이 된다.
        // 따라서 직접 회전을 시켜주어야할 필요가 있다.
        // Focus 상태일 경우는 focus 대상 주시
        // 
        private void NavAnimUpdate()
        {
            float speed = _nav.velocity.magnitude;
            // Debug.Log($"Desired Velocity : {_nav.desiredVelocity.magnitude}");
            // Debug.Log($"Velocity : {_nav.velocity.magnitude}");
            // if(_controller.IsFocusTarget)
            // {
            //     Vector3 target = _controller.PersonalTarget - transform.position;
            //     target.y = 0;
            //     angle = Vector3.SignedAngle(transform.forward, target, transform.up);
            //     target.Normalize();
            //     Quaternion targetRotation = Quaternion.LookRotation(target);
            //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _controller.GeneralStats.StrafTurnSpeed * Time.deltaTime);
            // }
            // else
            // {
            //     if(_controller.Nav.desiredVelocity == Vector3.zero)
            //     {
            //         angle = 0.0f;
            //     }
            //     else
            //     {
            //         angle = Vector3.SignedAngle(transform.forward, _controller.Nav.desiredVelocity, transform.up);
            //     }
            // }

            // @To-DO
            // Last Direction 로직 수정할 것
            if(_controller.IsFocusTarget)
            {
                Vector3 target = _controller.PersonalTarget - transform.position;
                target.y = 0;
                UpdateRot(target);
            }
            else if(_controller.Nav.desiredVelocity != Vector3.zero)
            {
                UpdateRot(_controller.Nav.desiredVelocity);
                lastDir = _controller.Nav.desiredVelocity;
            }
            else
            {
                if(lastDir != Vector3.zero)
                {
                    UpdateRot(lastDir);
                }
            }
            _animator.SetFloat(AnimatorKey.Enemy.Speed, speed, _controller.GeneralStats.SpeedDampTime, Time.deltaTime);
        }

        private void UpdateRot(Vector3 faceDir)
        {
            faceDir.Normalize();
            faceDir.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            targetRot = Quaternion.Slerp(transform.rotation, targetRot, _controller.GeneralStats.AngularSpeedDampFactor * Time.deltaTime);

            float maxRotAngle = _controller.GeneralStats.MaxAngularSpeed * Time.deltaTime;
            Quaternion finalRot = Quaternion.RotateTowards(transform.rotation, targetRot, maxRotAngle);

            _angularSpeed = Quaternion.Angle(transform.rotation, finalRot);
            transform.rotation = finalRot;
        }
    }
}

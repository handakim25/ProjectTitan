using System.Collections;
using System.Collections.Generic;
using Titan.Audio;
using UnityEngine;

namespace Titan.Character.Player
{
    /// <summary>
    /// 기본적인 Locomotion을 관리하는 행동이다.
    /// DefaultBehaviour로 등록되므로 다른 행동이 끝나면 이곳으로 돌아온다.
    /// Jump, Walk, Run, Fall 등을 처리한다.
    /// 일단은 3가지 상태가 있다.
    /// 이동(지상)
    /// 점프(올라가고 내려가고)
    /// Fall(갑자기 떨어지는 경우)
    /// </summary>
    public class MoveBehaviour : GenericBehaviour
    {
        #region Variables

        [Header("Locomotion")]
        [Tooltip("달리기 속도")]
        [SerializeField] private float RunSpeed = 5f;
        [Tooltip("걷기 속도")]
        [SerializeField] private float WalkSpeed = 3f;
        [Tooltip("가속도")]
        [SerializeField] private float SpeedAccelaration = 20f;
        [Tooltip("회전 Damp")]
        [SerializeField] private float TurnSmoothingDamp = 20f;
        
        [Header("Jump")]
        [Tooltip("Jump 시작 시에 상승 속도")]
        [SerializeField] private float JumpForce = 5f;
        [Tooltip("Jump 시작 시에 전진 속도")]
        [SerializeField] private float JumpForwardSpeed = 5f;
        [SerializeField] private float JumpDeacceleration = 5f;
        [Tooltip("Jump 시에 재생할 사운드")]
        [SerializeField] private SoundList _jumpSfx = SoundList.None;

        /// <summary>
        /// Jump 중인지 여부
        /// </summary>
        private bool _isJumping = false;

        #endregion Variables

        private void Start()
        {
            _controller.SubscribeGenericBehaviour(this);
            _controller.RegisterDefaultBehaviour(BehaviourCode);

            // Callbacks
            _controller.PlayerInput.OnJumpPerformed += OnJumpPerformedHandler;
            _controller.OnGroundEnter += OnGroundEnterHandler;
            _controller.OnGroundExit += OnGroundExitHandler;

            _controller.ApplyGravity = true;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.red);
            // Debug.Log($"name : {hit.collider.name} / hit normal : {hit.normal} / frame count : {Time.frameCount}");
        }

        // 이동에 관한 정리 사항
        // 1. Speed
        // 이동 판정의 경우 적절한 보간이 있어야 중간에 IsMoving이 False가 되지 않는다.
        // 2. Rotation
        public override void LocalUpdate()
        {
            if(_controller.IsGround && !_isJumping)
            {
                _controller.PlayerMove.ResetFall();
                PlaneMove();
            }
            else if(!_controller.IsGround)
            {
                // In Air
            }
        }

        public override void OnEnter()
        {
            _controller.ApplyGravity = true;
            _controller.PlayerMove.Speed = 0f;
        }

        public override void OnExit()
        {
            // MoveSpeed를 초기화해야 다시 돌아왔을 때 제대로 보간된다.
            // 만약에 빠른 속도에서 보간되어야 한다면 해당 행동에서 처리한다.
            _controller.Animator.SetFloat(AnimatorKey.Player.MoveSpeed, 0.0f);
        }

        /// <summary>
        /// 지상에서 이동. 이동에 대한 처리는 PlayerMove에서 한다.
        /// </summary>
        private void PlaneMove()
        {
            // Calculate Target Speed
            // 이동 없을 경우 Target Speed는 0이 된다.
            float targetSpeed = _controller.Controller.IsWalk ? WalkSpeed : RunSpeed;
            targetSpeed = _controller.PlayerInput.MoveDir == Vector2.zero ? 0 : targetSpeed;
            _controller.PlayerMove.Speed = CalculateSpeed(targetSpeed, _controller.PlayerMove.Speed);

            // Rotating
            Vector3 moveDir = _controller.GetCameraFaceDir();
            _controller.PlayerMove.MoveDir = moveDir;
            _controller.FaceDirection(moveDir, false, TurnSmoothingDamp);
            _controller.SetLastDirection(moveDir); // @Refator : 입력 처리는 일괄적으로 해야한다. 이런 식이면 다른 비슷한 코드에서도 관리해야 된다.

            // Update Animation
            _controller.Animator.SetFloat(AnimatorKey.Player.MoveSpeed, _controller.PlayerMove.Speed);
        }

        /// <summary>
        /// TargetSpeed를 기반으로 현재 속도를 계산한다. 속도 계산은 선형 보간을 이용
        /// </summary>
        /// <param name="targetSpeed">목표 속도</param>
        /// <param name="curSpeed">현재 속도</param>
        /// <param name="changeImmedatie">속도를 즉각적으로 변화</param>
        /// <returns></returns>
        protected float CalculateSpeed(float targetSpeed, float curSpeed, bool changeImmedatie = false)
        {
            if(curSpeed < targetSpeed - 0.1f || curSpeed > targetSpeed + 0.1f)
            {
                return Mathf.Lerp(curSpeed, targetSpeed, Time.deltaTime * SpeedAccelaration);
            }
            else
            {
                return targetSpeed;
            }
        }

        #region Callbacks
        
        /// <summary>
        /// Jump 입력이 들어왔을 때 처리, 지상에서만 점프 가능
        /// </summary>
        private void OnJumpPerformedHandler()
        {
            if(!_controller.IsCurrentBehaviour(BehaviourCode))
            {
                Debug.Log($"Not current");
                return;
            }

            if(_controller.IsGround)
            {
                // Jump 상태로 설정하고,
                // 지상 속도, Y축 속도를 설정
                // 이동 방향으로 회전
                _isJumping = true;
                _controller.PlayerMove.SetYSpeed(JumpForce);
                _controller.PlayerMove.Speed = JumpForwardSpeed;
                _controller.FaceDirection(_controller.GetLastDirection(), true);

                _controller.Animator.SetBool(AnimatorKey.Player.IsJump, true);

                SoundManager.Instance.PlayEffectSound((int)_jumpSfx);
            }
        }

        /// <summary>
        /// Ground에 닿았을 때 처리, 이동 속도를 초기화, Jump 상태를 False로 설정
        /// </summary>
        private void OnGroundEnterHandler()
        {
            _controller.PlayerMove.Speed = 0;
            _isJumping = false;

            _controller.Animator.SetBool(AnimatorKey.Player.IsJump, false);
        }

        // Fall or Jump
        private void OnGroundExitHandler()
        {
            // Fall
            // if(!startJump)
            // {
            //     _controller.PlayerMove.ResetFall();
            // }
        }
        
        #endregion Callbacks
    }
}

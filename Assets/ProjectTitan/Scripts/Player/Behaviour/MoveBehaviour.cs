using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float RunSpeed = 5f;
        [SerializeField] private float WalkSpeed = 3f;
        [SerializeField] private float SpeedAccelaration = 20f;
        [SerializeField] private float TurnSmoothingDamp = 20f;
        
        [Header("Jump")]
        [SerializeField] private float JumpForce = 5f;
        [SerializeField] private float JumpForwardSpeed = 5f;
        [SerializeField] private float JumpDeacceleration = 5f;

        /// <summary>
        /// Jump 진행 중인지
        /// </summary>
        private bool startJump = false;

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
            if(_controller.IsGround && !startJump)
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
            _controller.Animator.SetFloat(AnimatorKey.Player.MoveSpeed, 0.0f);
        }

        private void PlaneMove()
        {
            // Calculate Target Speed
            float targetSpeed = _controller.Controller.IsWalk ? WalkSpeed : RunSpeed;
            targetSpeed = _controller.PlayerInput.MoveDir == Vector2.zero ? 0 : targetSpeed;
            _controller.PlayerMove.Speed = CalculateSpeed(targetSpeed, _controller.PlayerMove.Speed);

            // Rotating
            Vector3 moveDir = _controller.GetCameraFaceDir();
            _controller.PlayerMove.MoveDir = moveDir;
            _controller.FaceDirection(moveDir, false, TurnSmoothingDamp);
            _controller.SetLastDirection(moveDir);

            // Update Animation
            _controller.Animator.SetFloat(AnimatorKey.Player.MoveSpeed, _controller.PlayerMove.Speed);
        }

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

        void FootR()
        {

        }

        void FootL()
        {
            
        }

        void Land()
        {

        }
        
        private void OnJumpPerformedHandler()
        {
            if(!_controller.IsCurrentBehaviour(BehaviourCode))
            {
                Debug.Log($"Not current");
                return;
            }

            if(_controller.IsGround)
            {
                startJump = true;
                _controller.PlayerMove.SetYSpeed(JumpForce);
                _controller.PlayerMove.Speed = JumpForwardSpeed;
                _controller.FaceDirection(_controller.GetLastDirection(), true);

                _controller.Animator.SetBool(AnimatorKey.Player.IsJump, true);
            }
        }

        // Land
        private void OnGroundEnterHandler()
        {
            _controller.PlayerMove.Speed = 0;
            startJump = false;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    /// <summary>
    /// Dash 행동
    /// </summary>
    public class DashBehaviour : GenericBehaviour
    {
        #region Varaibles
        
        [SerializeField] private float _dashSpeed = 5f;
        [SerializeField] private float _dashDuration = 1f;
        [SerializeField] private AnimationCurve _dashSpeedCurve;
        
        private float _dashTime = 0f;

        #endregion Varaibles

        private void Start()
        {
            _controller.SubscribeGenericBehaviour(this);

            _controller.PlayerInput.OnDashPerformed += OnDashPerformedHandler;
            _controller.OnGroundExit += OnGroundExitHandler;
        }

        public override void LocalUpdate()
        {
            // Move
            _dashTime += Time.deltaTime;
            if(_dashTime > _dashDuration)
            {
                _controller.UnregisterBehaviour(BehaviourCode);
            }

            if(_controller.IsGround)
            {
                _controller.PlayerMove.ResetFall();
            }
            _controller.PlayerMove.Speed = _dashSpeed * _dashSpeedCurve.Evaluate(_dashTime / _dashDuration);
        }

        public override void OnEnter()
        {
            // Player Input이 있으면 Input 방향으로 이동하고
            // 없을 경우 마지막 방향으로 이동한다.
            if(_controller.PlayerInput.MoveDir != Vector2.zero)
            {
                _controller.PlayerMove.MoveDir = _controller.GetCameraFaceDir();
            }
            else
            {
                Vector3 moveDir = _controller.transform.forward;
                moveDir.y = 0f;
                _controller.PlayerMove.MoveDir = moveDir;
            }
            _controller.FaceDirection(_controller.PlayerMove.MoveDir, true);

            _dashTime = 0f;
            _controller.Animator.SetTrigger(AnimatorKey.Player.DashTrigger);
            _controller.Animator.SetBool(AnimatorKey.Player.IsDash, true);
        }

        public override void OnExit()
        {
            _controller.Animator.SetBool(AnimatorKey.Player.IsDash, false);
        }

        #region Callbacks
        
        public void OnDashPerformedHandler()
        {
            if(_controller.IsGround)
            {
                _controller.RegisterBehaviour(BehaviourCode);
            }
        }

        public void OnGroundExitHandler()
        {
            // _controller.UnregisterBehaviour(BehavriouCode);
        }
        
        #endregion Callbacks
    }
}

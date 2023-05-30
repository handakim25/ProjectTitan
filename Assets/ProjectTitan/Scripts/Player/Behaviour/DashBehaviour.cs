using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
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

            _controller.PlayerMove.Speed = _dashSpeed * _dashSpeedCurve.Evaluate(_dashTime / _dashDuration);
        }

        public override void OnEnter()
        {
            if(_controller.PlayerInput.MoveDir != Vector2.zero)
            {
                _controller.PlayerMove.MoveDir = _controller.GetCameraFaceDir();
            }
            else
            {
                _controller.PlayerMove.MoveDir = transform.forward;
            }
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

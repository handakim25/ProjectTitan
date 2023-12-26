using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Audio;

namespace Titan.Character.Player
{
    /// <summary>
    /// Dash 행동, 지정된 시간 동안, 지정된 속도로 대시한다.
    /// </summary>
    public class DashBehaviour : GenericBehaviour
    {
        #region Varaibles
        
        [Tooltip("대시 속도")]
        [SerializeField] private float _dashSpeed = 5f;
        [Tooltip("대시 지속 시간")]
        [SerializeField] private float _dashDuration = 1f;
        [Tooltip("대시 속도 곡선, x축은 시간, y축 1은 DashSpeed")]
        [SerializeField] private AnimationCurve _dashSpeedCurve;
        [Tooltip("대시 사운드")]
        [SerializeField] private SoundList _dashSfx = SoundList.None;
        
        /// <summary>
        /// 현재 진행 중긴 대시 시간
        /// </summary>
        private float _dashTimer = 0f;

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
            _dashTimer += Time.deltaTime;
            if(_dashTimer > _dashDuration)
            {
                _controller.UnregisterBehaviour(BehaviourCode);
            }

            if(_controller.IsGround)
            {
                _controller.PlayerMove.ResetFall();
            }
            _controller.PlayerMove.Speed = _dashSpeed * _dashSpeedCurve.Evaluate(_dashTimer / _dashDuration);
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

            _dashTimer = 0f;
            _controller.Animator.SetTrigger(AnimatorKey.Player.DashTrigger);
            _controller.Animator.SetBool(AnimatorKey.Player.IsDash, true);

            SoundManager.Instance.PlayEffectSound((int)_dashSfx);
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
            // 현재 구현 상에서는 그냥 지정된 시간 동안 대시하는 것으로 정해져 있다.
            // _controller.UnregisterBehaviour(BehavriouCode);
        }
        
        #endregion Callbacks
    }
}

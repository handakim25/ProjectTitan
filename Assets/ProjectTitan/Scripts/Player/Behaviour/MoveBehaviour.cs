using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    /// <summary>
    /// 기본적인 Locomotion을 관리하는 행동이다.
    /// DefaultBehaviour로 등록되므로 다른 행동이 끝나면 이곳으로 돌아온다.
    /// Jump, Walk, Run 등을 처리한다.
    /// Dash를 포함할지 여부는 추후에 처리할 생각이다.
    /// Fall 등의 처리도 한다.
    /// </summary>
    public class MoveBehaviour : GenericBehaviour
    {
        #region Variables
        
        [field : SerializeField] public float RunSpeed {get; private set;} = 5f;
        [field : SerializeField] public float WalkSpeed {get; private set;} = 3f;
        
        #endregion Variables
        private void Start()
        {
            _controller.SubscribeGenericBehaviour(this);
            _controller.RegisterDefaultBehaviour(BehavriouCode);
        }

        public override void LocalUpdate()
        {
            // Calculate Target Speed
            float targetSpeed = _controller.Controller.IsWalk ? WalkSpeed : RunSpeed;
            targetSpeed = _controller.PlayerInput.MoveDir == Vector2.zero ? 0 : targetSpeed;
            _controller.PlayerMove.Speed = targetSpeed;

            // Calculate Direction
            Vector3 moveDir = CalculateFaceDir();
            _controller.PlayerMove.MoveDir = moveDir;

            // Update Animation
            _controller.Animator.SetBool(AnimatorKey.Player.IsMoving, targetSpeed == 0 ? false : true);
        }

        private Vector3 CalculateFaceDir()
        {
            Transform cameraTr = _controller.Camera.transform;
            // Direction from camera
            // World 기준에서의 Transform의 forward를 구한다.
            Vector3 cameraForward = new(cameraTr.forward.x, 0, cameraTr.forward.z);
            Vector3 cameraRigth = new(cameraTr.right.x, 0, cameraTr.right.z);

            // MoveDir.x : ad Input, go left or right            
            // MoveDir.y : ws Input, go forward or backward
            // PlayerInput에서 Normalize된 상태로 넘어오기 때문에 대각성 이동은 문제 없다.
            return cameraForward.normalized * _controller.PlayerInput.MoveDir.y + cameraRigth.normalized * _controller.PlayerInput.MoveDir.x;
        }

        void FootR()
        {

        }

        void FootL()
        {
            
        }
    }
}

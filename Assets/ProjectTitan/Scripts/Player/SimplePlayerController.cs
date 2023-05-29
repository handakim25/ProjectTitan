using UnityEngine;
using UnityEngine.InputSystem;

using Titan.Core;

namespace Titan.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerController : MonoBehaviour, MainAction.IPlayerActions
    {
        [SerializeField] private InputActionAsset _inputAsset;
        private Animator _animator;
        private CharacterController _controller;
        private MainAction _action;
        private Transform _cameraTr;

        private Vector2 moveDir;
        private Vector3 faceDir;
        
        private void Awake()
        {
            _cameraTr = Camera.main.transform;
            _animator = GetComponent<Animator>();
            _action = new MainAction(_inputAsset);
            _action.Player.SetCallbacks(this);
            _action.Player.Enable();
            _controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
           bool isWalk = moveDir == Vector2.zero ? false : true;
           _animator.SetBool("IsWalk", isWalk);

            FaceMoveDirection();
            Move();
        }

        protected void FaceMoveDirection()
        {
            Vector3 cameraForward = new Vector3(_cameraTr.forward.x, 0, _cameraTr.forward.z);
            Vector3 cameraRight = new Vector3(_cameraTr.right.x, 0, _cameraTr.right.z);

            faceDir = cameraForward.normalized * moveDir.y + cameraRight.normalized * moveDir.x;
            if(faceDir == Vector3.zero)
                return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(faceDir), 10f * Time.deltaTime);
        }

        protected void Move()
        {
            Vector3 curMoveDir = new Vector3(faceDir.x, 0, faceDir.z);
            _controller.Move(curMoveDir * Time.deltaTime * 2.0f);   
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveDir = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            var mouseDelat = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

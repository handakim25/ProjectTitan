using UnityEngine;
using UnityEngine.InputSystem;

using Titan.Core;
using Titan.UI;

namespace Titan.Character.Player
{
    public class PlayerInput : MonoBehaviour, MainAction.IPlayerActions
    {
        [SerializeField] private InputActionAsset _asset;
        private MainAction _action;

        // @refactor
        // To show data
        // Replace with editor code later.
        [field : SerializeField] public Vector2 MoveDir {get; private set;}

        public event System.Action OnJumpPerformed;
        public event System.Action OnDashPerformed;
        public event System.Action OnBasicPerformed;
        public event System.Action OnSkillPerformed;
        public event System.Action OnHyperPerformed;

        private void Awake()
        {
            _action = new MainAction(_asset);
            _action.Player.SetCallbacks(this);
            // Fix
            // 로딩 중에는 움직여서는 안 된다.
            _action.Player.Disable();
        }

        #region Callbacks
        
        void MainAction.IPlayerActions.OnLook(InputAction.CallbackContext context)
        {
            // throw new System.NotImplementedException();
        }

        void MainAction.IPlayerActions.OnMove(InputAction.CallbackContext context)
        {
            MoveDir = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Debug.Log($"Context : {context.phase}");
            if(context.performed)
            {
                OnJumpPerformed?.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnDashPerformed?.Invoke();
            }
        }

        public void OnBasic(InputAction.CallbackContext context)
        {
            var device = InputSystem.GetDevice<Pointer>();
            if(context.performed && UIManager.Instance.IsRaycastHitTargetUI(device.position.ReadValue()) == false)
            {
                OnBasicPerformed?.Invoke();
            }
        }        

        public void OnSkill(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnSkillPerformed?.Invoke();
            }
        }

        public void OnHyper(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnHyperPerformed?.Invoke();
            }
        }
        
        #endregion Callbacks

        // @refactor
        // Player 입력을 제어할 수 있는 부분을 제한할 것
        // 여러곳에서 접근할 수 있으면 문제가 발생할 수 있다.
        public bool InputEnable
        {
            set
            {
                if(value)
                {
                    _action.Player.Enable();
                }
                else
                {
                    _action.Player.Disable();
                }
            }
        }
    }
}

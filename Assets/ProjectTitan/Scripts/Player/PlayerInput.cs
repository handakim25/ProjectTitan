using UnityEngine;
using UnityEngine.InputSystem;

using Titan.Core;

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

        private void Awake()
        {
            _action = new MainAction(_asset);
            _action.Player.SetCallbacks(this);
            _action.Player.Enable();
        }

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
            if(context.started)
            {
                OnJumpPerformed?.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnDashPerformed?.Invoke();
            }
        }
    }
}

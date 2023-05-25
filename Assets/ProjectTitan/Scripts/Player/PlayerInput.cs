using System.Collections;
using System.Collections.Generic;
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
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Titan.Core;
using Titan.InventorySystem.Items;

namespace Titan.UI
{
    public sealed class UIManager : MonoSingleton<UIManager>, MainAction.IUIActions
    {
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;
        // @Refactor
        // Change to scalable design latter
        [SerializeField] private UIScene inventoryUI;

        [SerializeField] private UIScene _HudScene;
        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();

        public event System.Action OnInteractEvent;
        public event System.Action<Vector2> OnInteractScrollEvent;

        #region Unity Methods
        
        private void Awake()
        {
            _action = new MainAction(_inputAsset);
            _action.UI.SetCallbacks(this);
            _action.UI.Enable();
        }

        private void Start()
        {
            ShowCursor();
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        private void OnApplicationFocus(bool focusStatus)
        {
            if(focusStatus)
            {
                HideCursor();
            }
        }
        
        #endregion Unity Methods

        // Memo
        // Think UI as Scene
        // Once one scene open -> other scenes should be closed
        // Each scene close themselves
        // UIManger should take care of other objects
        // First thing first.
        // Set parent ? register?
        // One UI should not know other UIs

        // UI calls -> SceneManager.ChangeScene
        // or
        // UI Open UI -> Callback UIManager

        public void OpenUIScene(UIScene targetScene)
        {
            foreach(UIScene scene in _UIList)
            {
                if(scene == targetScene)
                {
                    continue;
                }
                scene.CloseUI();
            }
        }

        public void CloseUISceneHandler()
        {
            _HudScene.OpenUI();
        }

        #region Input Callback
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnInteractEvent?.Invoke();
            }
        }

        public void OnInteractScroll(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                Vector2 scroll = context.ReadValue<Vector2>();
                OnInteractScrollEvent?.Invoke(scroll);
            }
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnShowCursor(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                ShowCursor();
            }
            else if(context.canceled)
            {
                HideCursor();
            }
        }

        #endregion Input Callback

        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // To-Do
        // 상황에 맞춰서 커서 숨김 여부 처리
        // Show
        // 1. Alt 키 누른 동안
        // 2. UI 활성화
        // 그 외 상황
        // Hide
        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

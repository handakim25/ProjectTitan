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

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            _action = new MainAction(_inputAsset);
            _action.UI.SetCallbacks(this);
            _action.UI.Enable();
        }

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

        #endregion Input Callback
    }
}

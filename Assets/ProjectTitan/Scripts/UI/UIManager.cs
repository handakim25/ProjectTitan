using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

using Titan.Core;
using Titan.Graphics.PostProcessing;

namespace Titan.UI
{
    public sealed class UIManager : MonoSingleton<UIManager>, MainAction.IUIActions
    {
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;
        // @Refactor
        // Change to scalable design latter
        [SerializeField] private UIScene inventoryUI;

        [Tooltip("HUD Scene. 기본 진행 중에는 HUD가 표시되고 다른 UI를 열었을 경우 닫히게 된다. 다른 UI가 닫혔을 경우 다시 열린다.")]
        [SerializeField] private UIScene _HudScene;
        // 굳이 직접 reference할 필요 없이 register pattern도 고려할 것
        [Tooltip("UI Scene List. 정상적으로 열리고 닫히기 위해서는 여기에 등록해야 한다.")]
        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();

        public event System.Action OnInteractEvent;
        public event System.Action<Vector2> OnInteractScrollEvent;

        private bool isMouseMode = false;

        public HudUIController HudUIController => _HudScene as HudUIController;

        #region Unity Methods
        
        private void Awake()
        {
            _action = new MainAction(_inputAsset);
            _action.UI.SetCallbacks(this);
            _action.UI.Enable();
        }

        private void Start()
        {
            HideCursor();
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        private void OnApplicationFocus(bool focusStatus)
        {
            if(focusStatus)
            {
                if(isMouseMode)
                {
                    ShowCursor();
                }
                else
                {
                    HideCursor();
                }
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

        // UI Scene들이 이것을 이용해서 자기 자신을 열게 한다.
        public void OpenUIScene(UIScene targetScene)
        {
            foreach(UIScene scene in _UIList)
            {
                if(scene == targetScene)
                {
                    continue;
                }
                if(scene.gameObject.activeSelf)
                {
                    scene.CloseUI();
                }
            }
            isMouseMode = true;
            ShowCursor();
            _action.Player.Disable();

            GameManager.Instance.PauseGame();
            BlurManager.Instance.BlurActive = true;
        }
        
        public void CloseUIScene(UIScene scene)
        {
            if(scene.gameObject.activeSelf)
            {
                scene.CloseUI();
            }
            isMouseMode = false;
            HideCursor();
            _action.Player.Enable();
            _HudScene.OpenUI();

            GameManager.Instance.ResumeGame();
            BlurManager.Instance.BlurActive = false;
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
                if(isMouseMode)
                {
                    ShowCursor();
                }
                else
                {
                    HideCursor();
                }
            }
        }

        #endregion Input Callback

        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            _action.Player.Look.Disable();
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
            _action.Player.Look.Enable();
        }

        #region Utillity
        
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new();
        /// <summary>
        /// Check if hits UI
        /// </summary>
        /// <param name="position">Screen Space Position</param>
        /// <returns>True if, hits ui</returns>
        public bool IsRaycastHitTargetUI(Vector2 position)
        {
            _pointerEventData ??= new PointerEventData(EventSystem.current);
            _pointerEventData.position = position;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            return _raycastResults.Any(result => result.gameObject.GetComponent<Button>() != null);
        }
        
        #endregion Utillity
    }
}

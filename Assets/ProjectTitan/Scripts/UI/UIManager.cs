using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

using Titan.Core;
using Titan.Graphics.PostProcessing;

namespace Titan.UI
{
    // @To-Do
    // Shortcut 구현
    public sealed class UIManager : MonoSingleton<UIManager>, MainAction.IUIActions
    {
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;
        // @Refactor
        // 현재는 각각의 UI Scene을 직접 reference하고 있다.
        // 이 부분은 Scalable하지 않다.
        // 1. UI Scene Data를 추가로 지정
        // 2. UI Scene이 자기 자신을 등록
        [SerializeField] private UIScene inventoryUI;

        [Tooltip("HUD Scene. 기본 진행 중에는 HUD가 표시되고 다른 UI를 열었을 경우 닫히게 된다. 다른 UI가 닫혔을 경우 다시 열린다.")]
        [SerializeField] private UIScene _HudScene;
        [Tooltip("UI Scene List. 정상적으로 열리고 닫히기 위해서는 여기에 등록해야 한다.")]
        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();
        public HudUIController HudUIController => _HudScene as HudUIController;

        /// <summary>
        /// 마우스 모드
        /// 1. UI가 열려있을 경우
        /// 2. Alt 키를 누른 경우
        /// Focus를 잃었을 경우에는 마우스 모드가 해제된다.
        /// Foucs를 얻을 경우에는 다시 조건을 확인한다.
        /// </summary>
        private bool isMouseMode = false;

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
            MouseModeOff();
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
                    MouseModeOn();
                }
                else
                {
                    MouseModeOff();
                }
            }
        }
        
        #endregion Unity Methods

        // @Memo
        // UI는 하나의 Scene이다.
        // 하나의 Scene이 열리게 되면 다른 Scene은 닫히게 된다.
        // 각 Scene은 자기 자신을 닫는다.
        // 각각의 Scene은 독립적이다.

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

            // Input 처리
            isMouseMode = true;
            MouseModeOn();
            _action.Player.Disable();

            if(targetScene.shouldTimeStop)
            {
                GameManager.Instance.PauseGame();
            }
            if(targetScene.shouldBlur)
            {
                BlurManager.Instance.BlurActive = true;
            }
        }
        
        public void CloseUIScene(UIScene scene)
        {
            if(scene.gameObject.activeSelf)
            {
                scene.CloseUI();
            }

            // Input 처리
            isMouseMode = false;
            MouseModeOff();
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
                MouseModeOn();
            }
            else if(context.canceled)
            {
                if(isMouseMode)
                {
                    MouseModeOn();
                }
                else
                {
                    MouseModeOff();
                }
            }
        }

        #endregion Input Callback

        public void MouseModeOn()
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
        public void MouseModeOff()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _action.Player.Look.Enable();
        }

        #region Utillity
        
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new();
        /// <summary>
        /// UI에 Raycast가 되는지 확인
        /// </summary>
        /// <param name="position">Screen Space Position</param>
        /// <returns>UI 검출 경우 True</returns>
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

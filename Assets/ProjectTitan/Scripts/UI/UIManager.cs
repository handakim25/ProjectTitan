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
    /// <summary>
    /// UI Scene을 관리하는 싱글톤 클래스
    /// </summary>
    sealed public class UIManager : MonoSingleton<UIManager>, MainAction.IUIActions
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;

        [Header("UI")]
        [Tooltip("HUD Scene. 기본 진행 중에는 HUD가 표시되고 다른 UI를 열었을 경우 닫히게 된다. 다른 UI가 닫혔을 경우 다시 열린다.")]
        public UIScene _HudScene;
        public UIScene inventoryUI;
        public UIScene _questUI;

        // @Refactor
        // UI Scene을 각 Scene이 등록을 하거나 아니면 UI Manager가 찾아서 등록하거나 하는 식으로 수정할 것

        /// <summary>
        /// 등록된 UI Scene List. 창이 열릴 때 다른 창은 닫히게 된다.
        /// </summary>
        [Space]
        [Tooltip("UI Scene List. 정상적으로 열리고 닫히기 위해서는 여기에 등록해야 한다.")]
        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();
        public HudUIController HudUIController => _HudScene as HudUIController;

        // HUD가 열려있지 않으면 다른 UI가 열려 있다.
        private bool IsUIOpen => !HudUIController.gameObject.activeSelf;

        /// <summary>
        /// 마우스 모드
        /// <para>1. UI가 열려있을 경우</para>
        /// <para>2. Alt 키를 누른 경우</para>
        /// Focus를 잃었을 경우에는 마우스 모드가 해제된다.
        /// Foucs를 얻을 경우에는 다시 조건을 확인한다.
        /// </summary>
        private bool isMouseMode = false;

        // @Refactor
        // Input 처리 방식을 다시 생각해볼 것

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
        /// Focus를 얻었을 때 isMouseMode에 따라 마우스 모드를 설정한다.
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

        private bool _isOpening = false;

        /// <summary>
        /// 해당 Scene을 열게 한다. 다른 Scene은 닫히게 된다. 마우스 모드가 활성화 되고 Player Input이 비활성화 된다.
        /// </summary>
        /// <param name="targetScene"></param>
        public void OpenUIScene(UIScene targetScene)
        {
            _isOpening = true;  
            foreach(UIScene scene in _UIList)
            {
                if(scene != targetScene && scene.gameObject.activeSelf)
                {
                    scene.CloseUI();
                }
            }

            // Input 처리
            isMouseMode = true;
            MouseModeOn();
            _action.Player.Disable();

            // UI 오픈 뒤의 효과 처리
            if(targetScene.shouldTimeStop)
            {
                GameManager.Instance.PauseGame();
            }
            if(targetScene.shouldBlur)
            {
                BlurManager.Instance.BlurActive = true;
            }
            _isOpening = false;
        }
        
        /// <summary>
        /// 해당 UIScene을 닫는다. 닫은 이후로는 HUD Scene을 열게 된고 마우스 모드를 종료한다.
        /// </summary>
        /// <param name="scene"></param>
        public void CloseUIScene(UIScene scene)
        {
            if(_isOpening)
            {
                return;
            }
            // OnEnable에서 HUD의 Animation이 진행된다.
            _HudScene.gameObject.SetActive(true);

            // Input 처리
            isMouseMode = false;
            MouseModeOff();
            _action.Player.Enable();

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
            if(context.performed)
            {
                ToggleUI(inventoryUI);
            }
        }

        public void OnQuest(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                ToggleUI(_questUI);
            }
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

        public void OnScreenshot(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                Debug.Log($"Status : {GameManager.Instance.Status}");
                if(GameManager.Instance.Status == GameStatus.Play || GameManager.Instance.Status == GameStatus.Pause)
                {
                    Utility.Screenshot.Capture();
                }
            }
        }

        #endregion Input Callback

        // Mouse Mode
        // 1. HUD 상태 : 마우스 모드를 해제한다. Alt 키를 누르면 마우스 모드를 활성화한다.
        // 2. UI 상태 : 마우스 모드를 설정한다.

        /// <summary>
        /// 마우스 모드를 활성화한다.
        /// </summary>
        public void MouseModeOn()
        {
            Cursor.lockState = CursorLockMode.None;
            _action.Player.Look.Disable();
        }

        /// <summary>
        /// 마우스 모드를 비활성화한다.
        /// </summary>
        public void MouseModeOff()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _action.Player.Look.Enable();
        }

        #region Utillity
        
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new();
        /// <summary>
        /// UI에 Raycast가 되는지 확인. Button은 제외한다.
        /// </summary>
        /// <param name="position">Screen Space Position</param>
        /// <returns>UI 검출 경우 True</returns>
        public bool IsRaycastHitTargetUI(Vector2 position)
        {
            _pointerEventData ??= new PointerEventData(EventSystem.current);
            _pointerEventData.position = position;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            return _raycastResults.Any(result => result.gameObject.GetComponent<Button>() != null || result.gameObject.GetComponent<TweenButton>() != null);
        }

        /// <summary>
        /// UI Scene을 열거나 닫는다.
        /// </summary>
        /// <param name="targetScene"></param>
        private void ToggleUI(UIScene targetScene)
        {
            if(targetScene == null)
            {
                Debug.LogError("Target Scene is Null");
                return;
            }

            if(targetScene.gameObject.activeSelf)
            {
                targetScene.CloseUI();
            }
            else
            {
                // OpenUIScene(scene);
                targetScene.OpenUI();
            }
        }
        #endregion Utillity
    }
}

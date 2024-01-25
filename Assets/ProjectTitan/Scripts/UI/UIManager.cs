using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

using Titan.Core;
using Titan.Graphics.PostProcessing;

// @Refactor
// OpenUI, CloseUI 수정할 것
// 1. UIScene에서 UIManager를 호출하는 것은 좋아 보이지 않는다.
// 2. CloseUI()를 호출하는 것이 아니라, UIScene에서 CloseUI를 호출하는 것도 좋아 보이지 않는다.
// 실수할 가능성이 높다. 아니면 메소드의 의미가 명확하지 않는 것으로 보인다.
// 3. 어쩌면 Input 처리도 각 Scene이 처리하는 것이 좋을 수도 있다. 가령, 겹치는 UI가 있을 경우에는 어떻게 처리할 것인가? 혹은 팝업 메뉴의 처리는? 가령 다른 게임에서는 toggle 시에 팝업을 닫는 방식으로 처리할 때도 있다.
// 이런 케이스를 UIManager에서 다 처리하기에는 어려움이 있을 수 있다.
// 각 Scene이 자신의 Input을 처리하고, UIManager에서는 각 Scene의 요청을 받아서 처리하는 것이 좋을 수도 있다.
// 그렇다면 Input을 어떻게 넘겨줄 것인가?

namespace Titan.UI
{
    /// <summary>
    /// UI Scene을 관리하는 싱글톤 클래스
    /// </summary>
    sealed public class UIManager : MonoSingleton<UIManager>, MainAction.IUIActions
    {
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;

        [Header("UI")]
        [Tooltip("HUD Scene. 기본 진행 중에는 HUD가 표시되고 다른 UI를 열었을 경우 닫히게 된다. 다른 UI가 닫혔을 경우 다시 열린다.")]
        [SerializeField] private UIScene _HudScene;
        [SerializeField] private UIScene inventoryUI;
        [SerializeField] private UIScene _questUI;

        // @Refactor
        // UI Scene을 각 Scene이 등록을 하거나 아니면 UI Manager가 찾아서 등록하거나 하는 식으로 수정할 것

        /// <summary>
        /// 등록된 UI Scene List. 창이 열릴 때 다른 창은 닫히게 된다.
        /// </summary>
        [Space]
        [Tooltip("UI Scene List. 정상적으로 열리고 닫히기 위해서는 여기에 등록해야 한다.")]
        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();
        public HudUIController HudUIController => _HudScene as HudUIController;

        [System.Serializable]
        public class RarityColor
        {
            public Color Common;
            public Color Uncommon;
            public Color Rare;
            public Color Epic;

            // 강조하기 위해서 사용하는 색상. 조금 더 어두운 색상.
            [Space]
            public Color Commonintense;
            public Color Uncommonintense;
            public Color Rareintense;
            public Color Epicintense;
        }

        [SerializeField] private RarityColor rarityColor;
        public RarityColor RarityColorTable => rarityColor;

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
                // CloseUIScene(scene);
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

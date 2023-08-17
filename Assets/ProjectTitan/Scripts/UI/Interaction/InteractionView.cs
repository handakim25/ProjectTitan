using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

using Titan.Interaction;
using Titan.Utility;

namespace Titan.UI.Interaction
{
    // @Think
    // Slot 객체가 필요한가?
    // 없이 작업할려니까 조금 불편한데
    // Note : Interact로 사라지는 것과 거리 밖으로 넘어가서 없어지는 것은 다르다.
    // @refactor
    // Selection 부분 개선할 것
    // 문제점1. 순서에 너무 민감하게 작동하고 있는 점
    // 문제점2. List가 과연 맞는 선택인가 하는 점

    /// <summary>
    /// Interaction Panel에서 View를 담당하는 클래스
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class InteractionView : MonoBehaviour
    {
        #region Varaibles
        
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GameObject _interactIcon;
        [SerializeField] private RectTransform _interactCursor;
        private Color _normalColor;
        [SerializeField] private Color _hightlightColor = Color.cyan;

        private ScrollRect _scrollRect;
        private RectTransform _contentRectTransform;
        // key : Interactable Objects, Value : Interact UI
        private Dictionary<GameObject, InteractionUI> _interactionUIs = new();
        [SerializeField] private GameObject _selectedSlot = null;
        public GameObject SelectedSlot => _selectedSlot;
        public int SlotCount => _interactionUIs.Count;
        public int ChildCount => _scrollRect.content.childCount;
        
        #endregion Varaibles

        #region Unity Methods
        
        private void Awake()
        {
            Assert.IsNotNull(_slotPrefab);
            _scrollRect = GetComponent<ScrollRect>();
            _contentRectTransform = _scrollRect.content.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _selectedSlot = null;
            _normalColor = _slotPrefab.GetComponent<Image>().color;
        }

        private void OnDisable()
        {
            
        }
        
        #endregion Unity Methods

        #region Methods
        
        public void AddSlot(GameObject[] interactObjects)
        {
            Transform parent = _scrollRect.content.transform;
            foreach(GameObject interactObject in interactObjects)
            {
                GameObject slotUI = CreateSlot(parent);
                
                SetInteractSlot(slotUI, interactObject.GetComponent<Interactable>());
                var interactionUI = slotUI.GetComponent<InteractionUI>();
                interactionUI.Interactable = interactObject;
                _interactionUIs[interactObject] = slotUI.GetComponent<InteractionUI>();
                slotUI.name += $"_{interactObject.name}";
            }

            if(_selectedSlot == null)
            {
                SelectSlot(parent.GetChild(0).gameObject);
            }
            else
            {
                SetCursorPos();
            }
            if(_interactIcon.activeSelf == false)
            {
                _interactIcon.SetActive(true);
            }
        }

        // Think
        // 유틸리티적으로 쓰는 함수는 클래스 상태에 의존하기 보다는 독립적으로 작동하도록 하자.
        private GameObject CreateSlot(Transform parent)
        {
            GameObject slotUI = Instantiate(_slotPrefab, parent);

            return slotUI;
        }

        /// <summary>
        /// Interactable에 맞춰서 Slot을 설정한다
        /// </summary>
        /// <param name="slotUI">Slot</param>
        /// <param name="interactable">Interactable target</param>
        private void SetInteractSlot(GameObject slotUI, Interactable interactable)
        {
            var interactText = slotUI.transform.Find("InteractText");
            if(interactText && interactText.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                text.text = interactable.name;
            }
        }

        public void RemoveSlot(GameObject[] interactObjects)
        {
            foreach(GameObject removedObject in interactObjects)
            {
                if(_interactionUIs[removedObject].gameObject == _selectedSlot)
                {
                    SelectSlot(null);
                }
                // SetCursorPos 계산을 위함
                _interactionUIs[removedObject].gameObject.SetActive(false);
                Destroy(_interactionUIs[removedObject].gameObject);
                _interactionUIs.Remove(removedObject);
            }

            if(_interactionUIs.Count == 0)
            {
                _interactIcon.SetActive(false);
            }
            SetCursorPos();
        }

        /// <summary>
        /// 해당 Slot을 선택한다.
        /// </summary>
        /// <param name="selectedSlot"></param>
        public void SelectSlot(GameObject selectedSlot)
        {
            if(selectedSlot == _selectedSlot)
            {
                SetCursorPos();
                return;
            }

            if(_selectedSlot != null)
            {
                if(_selectedSlot.TryGetComponent<Image>(out var prevImage))
                {
                    prevImage.color = _normalColor;
                }
            }

            _selectedSlot = selectedSlot;
            if(_selectedSlot != null && _selectedSlot.TryGetComponent<Image>(out var image))
            {
                image.color = _hightlightColor;
            }
            SetCursorPos();

        }

        /// <summary>
        /// Interaction Cursor의 위치를 Update해주는 함수.
        /// 만약에 선택된 slot이 없다면 Marker를 비활성화한다.
        /// </summary>
        public void SetCursorPos()
        {
            if(_selectedSlot == null)
            {
                _interactCursor.gameObject.SetActive(false);
                return;
            }
            else
            {
                _interactCursor.gameObject.SetActive(true);
            }

            // Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);
            
            // var size = (Vector2)_scrollRect.transform.InverseTransformPoint(_scrollRect.content.position) - (Vector2)_scrollRect.transform.InverseTransformPoint(_selectedSlot.GetComponent<RectTransform>().position);
            var size = (Vector2)_scrollRect.transform.InverseTransformPoint(_selectedSlot.GetComponent<RectTransform>().position) - (Vector2)_scrollRect.transform.InverseTransformPoint(_scrollRect.content.position);
            _interactCursor.anchoredPosition = new Vector2(_interactCursor.anchoredPosition.x, size.y);
        }

        public GameObject GetSlotUIByIndex(int index)
        {
            if(index < 0)
            {
                Debug.Log($"index invalid");
                Debug.Log($"index : {index}");
                Debug.Log($"Count : {SlotCount}");
            }
            if(_scrollRect.content.transform.childCount <= index)
            {
                Debug.Log($"Index invalid");
                Debug.Log($"index : {index}");
                Debug.Log($"Count : {SlotCount}");
            }
            return _scrollRect.content.transform.GetChild(index).gameObject;
        }

        public bool IsValidSlot(GameObject slotUI)
        {
            if(slotUI == null)
                return false;
            GameObject slotInteracObject = slotUI.GetComponent<InteractionUI>().Interactable;
            return _interactionUIs.ContainsKey(slotInteracObject);
        }

        #endregion Methods
    }
}

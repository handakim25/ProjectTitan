using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using TMPro;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
    // @Further-Work
    // 1. 처음부터 비활성화 시에 OnEnable 호출 순서가 바뀌는 문제로 객체 참조 문제가 발생. 안정적으로 동작하도록 수정할 것. Start로 일부 함수로 옮겨서 해결
    // 1-2. 혹은 Interaction Controller에서 Interactoin View를 생성한다.
    // 2. Slot을 매번 생성할 필요 없이 Object Pooling으로 수정

    /// <summary>
    /// Interaction Panel에서 View를 담당하는 클래스
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class InteractionView : MonoBehaviour
    {
        #region Varaibles
        
        [Header("Interaction Slot")]
        [Tooltip("Interaction 하나를 표현하는 Slot Prefab")]
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Sprite _dialogueIcon;
        [SerializeField] private Sprite _pickupIcon;
        [SerializeField] private Sprite _useIcon;

        [Header("Interaction Icon")]
        [Tooltip("Interaction이 가능한 상태일 때 표시되는 Icon")]
        [SerializeField] private GameObject _interactIconObject;
        private RectTransform _interactIconRect;
        [Tooltip("Interaction Icon의 위치 Offset. Interaction View의 좌상단을 기준으로 한다.")]
        [SerializeField] private float _interactIconOffset = 0f;
        [Tooltip("현재 선택된 Slot을 가리키는 Cursor")]
        [SerializeField] private RectTransform _interactCursor;

        private ScrollRect _scrollRect;
        private RectTransform _contentRectTransform;
        /// <summary>
        /// Key : Interactable Objects, Value : Interact UI.
        /// Slot UI는 InteractionUI를 참조해서 접근할 것
        /// Remove Slot을 할 때 Interactable로 넘어오기 때문에 이를 이용해서 삭제한다.
        /// </summary>
        private Dictionary<Interactable, InteractionUI> _interactionUIs = new();
        [SerializeField] private GameObject _selectedSlot = null;
        public GameObject SelectedSlot => _selectedSlot;
        public int SlotCount => _interactionUIs.Count;
        public int ChildCount => _scrollRect != null ? _scrollRect.content.childCount : 0;
        
        #endregion Varaibles

        #region Unity Methods
        
        private void Awake()
        {
            Assert.IsNotNull(_slotPrefab);
            _scrollRect = GetComponent<ScrollRect>();
            _contentRectTransform = _scrollRect.content.GetComponent<RectTransform>();
            _interactIconRect = _interactIconObject.GetComponent<RectTransform>();
        }
        
        #endregion Unity Methods

        #region Methods
        
        /// <summary>
        /// Interaction Slot을 추가한다.
        /// </summary>
        /// <param name="interactObjects"></param>
        public void AddSlots(Interactable[] interactObjects)
        {
            Transform parent = _scrollRect.content.transform;
            foreach(Interactable interactObject in interactObjects)
            {
                GameObject slotUI = CreateSlot(parent);
                
                // @Refactor
                // InteractionUI가 필요 없을 수도 있다. 간략화시킬 수 있는 방향이 있다면 고려할 것
                InitInteractSlot(slotUI, interactObject.GetComponentInParent<Interactable>());
                var interactionUI = slotUI.GetComponent<InteractionUI>();
                interactionUI.Interactable = interactObject;
                _interactionUIs[interactObject] = interactionUI;
                slotUI.name += $"_{interactObject.name}";
            }

            // 비어있는 상태에서 처음 슬롯을 추가할 때
            if(_selectedSlot == null)
            {
                for(int i = 0; i< parent.childCount; i++)
                {
                    GameObject slotUI = parent.GetChild(i).gameObject;
                    if(IsValidSlot(slotUI))
                    {
                        SelectSlot(slotUI);
                        break;
                    }
                }
            }
            // 기존 슬롯이 있을 때
            else
            {
                UpdateCursorPos();
            }

            if(_interactIconObject.activeSelf == false)
            {
                _interactIconObject.SetActive(true);
            }
        }

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
        private void InitInteractSlot(GameObject slotUI, Interactable interactable)
        {
            if(interactable == null)
            {
                Debug.LogError("Interactable must attached to GameObject");
                return;
            }

            var interactText = slotUI.transform.Find("InteractionSlotBody/InteractText");
            if(interactText && interactText.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                text.text = interactable.InteractText;
            }

            var interactIcon = slotUI.transform.Find("InteractionSlotBody/InteractIcon");
            if(interactIcon && interactIcon.TryGetComponent<Image>(out var image))
            {
                image.sprite = interactable switch {
                    ItemInteractable => _pickupIcon,
                    DialogInteractable => _dialogueIcon,
                    EventTriggerInteractable => _useIcon,
                    _ => _useIcon,
                };
            }
        }

        public void RemoveSlot(Interactable[] interactObjects)
        {
            foreach(Interactable removedObject in interactObjects)
            {
                if(_interactionUIs[removedObject].gameObject == _selectedSlot)
                {
                    // Debug.Log($"Reset selected slot");
                    SelectSlot(null);
                }
                // SetCursorPos 계산을 위함
                _interactionUIs[removedObject].gameObject.SetActive(false);
                // Destroy 시점은 Update 이후에 이루어진다. 이번 Update 루프에서 삭제되는 것은 아니다.
                // 따라서 InteractionUIs의 값들을 참조할 것
                Destroy(_interactionUIs[removedObject].gameObject);
                _interactionUIs.Remove(removedObject);
            }

            if(_interactionUIs.Count == 0)
            {
                _interactIconObject.SetActive(false);
            }
            UpdateCursorPos();
        }

        /// <summary>
        /// 해당 Slot을 선택한다.
        /// </summary>
        /// <param name="selectedSlot"></param>
        public void SelectSlot(GameObject selectedSlot)
        {
            if(selectedSlot == _selectedSlot)
            {
                UpdateCursorPos();
                return;
            }

            // 기존 선택된 SLot이 있다면 Slot의 색을 기본색으로 변경
            if(_selectedSlot != null && _selectedSlot.TryGetComponent<InteractionUI>(out var prevInteractionUI))
            {
                prevInteractionUI.Deselect();
            }

            // Slot을 선택하고 색을 변경
            _selectedSlot = selectedSlot;
            if(_selectedSlot != null && _selectedSlot.TryGetComponent<InteractionUI>(out var interactionUI))
            {
                interactionUI.Select();
            }
            UpdateCursorPos();
        }

        /// <summary>
        /// Interaction Cursor의 위치를 Update해주는 함수.
        /// 만약에 선택된 slot이 없다면 Marker를 비활성화한다.
        /// </summary>
        public void UpdateCursorPos()
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

            // Layout 완료 시점의 Callback은 존재하지 않기 때문에 강제로 Update를 하거나 Coroutine을 이용해야 한다.
            // 현재는 강제로 Update를 호출한다.
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);
            
            var size = (Vector2)_scrollRect.transform.InverseTransformPoint(_selectedSlot.GetComponent<RectTransform>().position) - (Vector2)_scrollRect.transform.InverseTransformPoint(_scrollRect.content.position);
            _interactCursor.anchoredPosition = new Vector2(_interactCursor.anchoredPosition.x, size.y);
        }

        private LayoutGroup _contentLayoutGroup;
        private LayoutGroup ContentLayoutGroup
        {
            get
            {
                if(_contentLayoutGroup == null)
                {
                    _contentLayoutGroup = _scrollRect.content.GetComponent<LayoutGroup>();
                }
                return _contentLayoutGroup;
            }
        }
        
        /// <summary>
        /// Guide Icon의 위치를 갱신한다. Layout 계산이 끝나지 않았을 경우 제대로 된 위치를 계산하지 못할 수 있으므로 Layout 계산이 끝난 후에 호출해야 한다.
        /// </summary>
        public void UpdateGuideIcon()
        {
            if(SlotCount == 0)
            {
                _interactIconObject.SetActive(false);
                return;
            }
            _interactIconObject.SetActive(true);

            // Content의 Child로 지정하지 않고 직접 위치를 계산하는 것은 Child 개수를 직접적으로 이용해서 관리하고 있는 부분이 있어서이다.
            // 따라서 현재는 Content의 Child로 지정하지 않고 직접 계산한다.
            Vector2 newPos = _interactIconRect.anchoredPosition;
            newPos.y = ContentLayoutGroup.preferredHeight + _interactIconOffset;
            _interactIconRect.anchoredPosition = newPos;
        }

        /// <summary>
        /// Child Index로 slot을 가져온다. 해당 Slot이 이번 루프에서 파괴됬을 경우 유효하지 않은 slot이다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetSlotUIByIndex(int index)
        {
            if(index < 0 || index >= _scrollRect.content.transform.childCount)
            {
                Debug.Log($"Index invalid");
                Debug.Log($"Index : {index}");
                Debug.Log($"Count : {SlotCount}");
            }
            return _scrollRect.content.transform.GetChild(index).gameObject;
        }

        /// <summary>
        /// 해당 Slot이 유효한지 검사한다. 유효하지 않다면 삭제된 Slot이다.
        /// </summary>
        /// <param name="slotUI"></param>
        /// <returns></returns>
        public bool IsValidSlot(GameObject slotUI)
        {
            if(slotUI == null)
                return false;
            Interactable slotInteracObject = slotUI.GetComponent<InteractionUI>().Interactable;
            return _interactionUIs.ContainsKey(slotInteracObject);
        }

        public void Clear()
        {
            foreach(var interactionUI in _interactionUIs.Values)
            {
                Destroy(interactionUI.gameObject);
            }
            _interactionUIs.Clear();
            _selectedSlot = null;
            _interactIconObject.SetActive(false);
        }

        #endregion Methods
    }
}

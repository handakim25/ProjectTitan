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
        
        [SerializeField] private GameObject _slotPrefab;
        [Tooltip("Interact Guide Icon")]
        [SerializeField] private GameObject _interactIconObject;
        [SerializeField] private Vector2 _interactIconOffset = Vector2.zero;
        [SerializeField] private RectTransform _interactCursor;
        private Color _normalColor;
        [FormerlySerializedAs("_hightlightColor")]
        [SerializeField] private Color _selectColor = Color.cyan;

        private ScrollRect _scrollRect;
        private RectTransform _contentRectTransform;
        /// <summary>
        /// Key : Interactable Objects, Value : Interact UI.
        /// Slot UI는 InteractionUI를 참조해서 접근할 것
        /// </summary>
        private Dictionary<GameObject, InteractionUI> _interactionUIs = new();
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

            _normalColor = _slotPrefab.GetComponent<Image>().color;
        }
        
        #endregion Unity Methods

        #region Methods
        
        public void AddSlots(GameObject[] interactObjects)
        {
            Transform parent = _scrollRect.content.transform;
            foreach(GameObject interactObject in interactObjects)
            {
                GameObject slotUI = CreateSlot(parent);
                
                // @Refactor
                // InteractionUI가 필요 없을 수도 있다. 간략화시킬 수 있는 방향이 있다면 고려할 것
                SetInteractSlot(slotUI, interactObject.GetComponentInParent<Interactable>());
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
                SetCursorPos();
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
        private void SetInteractSlot(GameObject slotUI, Interactable interactable)
        {
            if(interactable == null)
            {
                Debug.LogError("Interactable must attached to GameObject");
                return;
            }

            var interactText = slotUI.transform.Find("InteractText");
            if(interactText && interactText.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                text.text = interactable.InteractText;
            }
        }

        public void RemoveSlot(GameObject[] interactObjects)
        {
            foreach(GameObject removedObject in interactObjects)
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
                // Debug.Log($"SelectSlot : {selectedSlot}");
                return;
            }

            // 기존 선택된 SLot이 있다면 Slot의 색을 기본색으로 변경
            if(_selectedSlot != null && _selectedSlot.TryGetComponent<Image>(out var prevImage))
            {
                prevImage.color = _normalColor;
            }

            // Slot을 선택하고 색을 변경
            _selectedSlot = selectedSlot;
            if(_selectedSlot != null && _selectedSlot.TryGetComponent<Image>(out var image))
            {
                image.color = _selectColor;
            }
            SetCursorPos();
        }

        /// <summary>
        /// Interaction Cursor의 위치를 Update해주는 함수.
        /// 만약에 선택된 slot이 없다면 Marker를 비활성화한다.
        /// </summary>
        // @Refactor
        // 호출 위치를 줄일 수 있을 것 같은데?
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

        public void UpdateGuideIcon()
        {
            if(SlotCount == 0)
            {
                _interactIconObject.SetActive(false);
                return;
            }
            _interactIconObject.SetActive(true);

            // LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

            GameObject firstGo = null;
            for(int i = 0; i < _scrollRect.content.childCount; ++i)
            {
                GameObject slotUI = _scrollRect.content.GetChild(i).gameObject;
                if(IsValidSlot(slotUI))
                {
                    firstGo = slotUI;
                    break;
                }
            }
            if (firstGo != null)
            {
                Vector3 position = firstGo.transform.position;
                _interactIconObject.transform.position = position;
                _interactIconObject.GetComponent<RectTransform>().anchoredPosition += _interactIconOffset;
                var firstRect = firstGo.transform as RectTransform;
            }
        }

        public GameObject GetSlotUIByIndex(int index)
        {
            if(index < 0)
            {
                Debug.Log($"Index invalid");
                Debug.Log($"Index : {index}");
                Debug.Log($"Count : {SlotCount}");
            }
            if(_scrollRect.content.transform.childCount <= index)
            {
                Debug.Log($"Index invalid");
                Debug.Log($"Index : {index}");
                Debug.Log($"Count : {SlotCount}");
            }
            return _scrollRect.content.transform.GetChild(index).gameObject;
        }

        /// <summary>
        /// 해당 Slot이 유효한지 검사한다.
        /// </summary>
        /// <param name="slotUI"></param>
        /// <returns></returns>
        public bool IsValidSlot(GameObject slotUI)
        {
            if(slotUI == null)
                return false;
            GameObject slotInteracObject = slotUI.GetComponent<InteractionUI>().Interactable;
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

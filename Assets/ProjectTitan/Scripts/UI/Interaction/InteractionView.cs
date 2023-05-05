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
    [RequireComponent(typeof(ScrollRect))]
    public class InteractionView : MonoBehaviour
    {
        #region Varaibles
        
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GameObject _interactIcon;
        private Color _normalColor;
        [SerializeField] private Color _hightlightColor = Color.cyan;

        private ScrollRect _scrollRect;
        // key : Interactable Objects, Value : Interact UI
        private Dictionary<GameObject, InteractionUI> _interactionUIs = new Dictionary<GameObject, InteractionUI>();
        private GameObject _selectedSlot = null;
        public GameObject SelectedSlot => _selectedSlot;
        public int SlotCount => _scrollRect.content.transform.childCount;
        
        #endregion Varaibles

        #region Unity Methods
        
        private void Awake()
        {
            Assert.IsNotNull(_slotPrefab);
            _scrollRect = GetComponent<ScrollRect>();
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
                Destroy(_interactionUIs[removedObject].gameObject);
                _interactionUIs.Remove(removedObject);
            }

            if(_interactionUIs.Count == 0)
            {
                _interactIcon.SetActive(false);
            }
        }

        public void SelectSlot(GameObject selectedSlot)
        {
            if(selectedSlot == _selectedSlot)
            {
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
        }

        public GameObject GetSlotUIByIndex(int index)
        {
            return _scrollRect.content.transform.GetChild(index).gameObject;
        }

        public bool IsValidSlot(GameObject slotUI)
        {
            GameObject slotInteracObject = slotUI.GetComponent<InteractionUI>().Interactable;
            return _interactionUIs.ContainsKey(slotInteracObject);
        }

        #endregion Methods
    }
}

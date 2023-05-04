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
    [RequireComponent(typeof(ScrollRect))]
    public class InteractionView : MonoBehaviour
    {
        #region Varaibles
        
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GameObject _interactIcon;
        private Color _normalColor;
        [SerializeField] private Color _hightlightColor = Color.cyan;

        private ScrollRect _scrollRect;
        // Gameobject : Interact Object, index : Sibling index of contents
        // Add / Remove -> remove from list and remove ui by index
        // interact select -> get interact object by sibling index
        private List<GameObject> _interactionList = new List<GameObject>();
        /// <summary>
        /// if _selectionIndex < 0 : unselected
        /// 0-base
        /// </summary>
        private int _selectedIndex = -1;
        
        #endregion Varaibles

        #region Unity Methods
        
        private void Awake()
        {
            Assert.IsNotNull(_slotPrefab);
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            _selectedIndex = -1;
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
                slotUI.name += $"_{interactObject.name}";
                _interactionList.Add(interactObject);
            }

            if(_selectedIndex < 0)
            {
                SelectSlot(0);
            }
            if(_interactionList.Count > 0)
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
            int startIndex = _selectedIndex;

            foreach(GameObject removedObject in interactObjects)
            {
                int removedIndex = _interactionList.FindIndex(interact => interact == removedObject);
                if(removedIndex == _selectedIndex)
                {
                    _selectedIndex = -1;
                }
                Destroy(_scrollRect.content.GetChild(removedIndex).gameObject);
            }

            foreach(GameObject removedObject in interactObjects)
            {
                _interactionList.Remove(removedObject);
            }

            if(_interactionList.Count == 0)
            {
                _interactIcon.SetActive(false);
            }
        }

        public void SelectSlot(int index)
        {
            if(index < 0 || _scrollRect.content.transform.childCount <= index)
            {
                return;
            }

            Transform content = _scrollRect.content.transform;
            if(_selectedIndex >= 0)
            {
                GameObject prevSelected = content.GetChild(_selectedIndex).gameObject;
                if(prevSelected.TryGetComponent<Image>(out var prevImage))
                {
                    prevImage.color = _normalColor;
                }
            }

            GameObject selectedSlot = content.GetChild(index).gameObject;
            if(selectedSlot.TryGetComponent<Image>(out var selectedImage))
            {
                selectedImage.color = _hightlightColor;
            }
        }
        
        #endregion Methods
    }
}

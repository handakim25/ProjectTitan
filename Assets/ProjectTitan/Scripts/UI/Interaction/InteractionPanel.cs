using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
    /// <summary>
    /// Interaction UI Controller
    /// </summary>
    public class InteractionPanel : MonoBehaviour
    {
        [SerializeField] private InteractionList _interactionList;
        [SerializeField] private InteractionView _view;

        #region Unity Methods
        
        private void OnEnable()
        {
            UIManager.Instance.OnInteractEvent += OnInteractHandler;
            UIManager.Instance.OnInteractScrollEvent += OnInteractScrollHandler;
            _interactionList.OnInteractChanged += InteractSlotChagned;

            _view.AddSlot(_interactionList.interactObjects.ToArray());
        }

        private void OnDisable()
        {
            if(UIManager.Instance != null)
            {
                UIManager.Instance.OnInteractEvent -= OnInteractHandler;
                UIManager.Instance.OnInteractScrollEvent -= OnInteractScrollHandler;
            }
            _interactionList.OnInteractChanged -= InteractSlotChagned;
        }

        /// <summary>
        /// InteractionList부터 Callback을 받는 함수.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void InteractSlotChagned(Object sender, InteractionList.InteractSlotChangedEventArgs args)
        {
            // Bug
            // Remove Slot에서 2개 이상의 Object를 Remove할 경우
            // Next Slot을 제대로 선택하지 못하는 문제가 있다.
            if(args.RemovedObjects != null)
            {
                if(_view.SelectedSlot == null)
                {
                    Debug.LogError($"Can this happen?");
                    Debug.Log($"Cur Slot Count : {_view.SlotCount}");
                    _view.SelectSlot(FindNextSelect(0));
                }
                int startIndex = _view.SelectedSlot.transform.GetSiblingIndex();
                _view.RemoveSlot(args.RemovedObjects);
                if(_view.SelectedSlot == null)
                {
                    Debug.Log($"Select Next");
                    _view.SelectSlot(FindNextSelect(startIndex));
                    Debug.Log($"Selected : {_view.SelectedSlot}");
                    if(_view.SelectedSlot == null && _view.SlotCount > 0)
                    {
                        Debug.LogError($"Failed to find next slot");
                    }
                }
                _view.SetCursorPos();
            }
            if(args.AddedObjects != null)
            {
                _view.AddSlot(args.AddedObjects);
            }
        }

        /// <summary>
        /// startIndex를 제외한 Slot을 선택한다.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private GameObject FindNextSelect(int startIndex)
        {
            // Object의 Update는 가장 마지막에 진행되기 때문에
            // Child에는 계속 존재하고 있다.
            Debug.Log($"----FindNextSlot");
            Debug.Log($"Start Index : {startIndex} / Count : {_view.SlotCount} / Child Count : {_view.ChildCount}");
            Debug.Log($"snapshot");
            for(int i = 0; i < _view.ChildCount; ++i)
            {
                GameObject interacionUI = _view.GetSlotUIByIndex(i);
                Debug.Log($"{i} : {interacionUI.name} : valid {_view.IsValidSlot(interacionUI)}");
            }
            Debug.Log($"----");
            Debug.Log($"After Check / Start Index : {startIndex + 1}");
            for(int i = startIndex + 1; i < _view.ChildCount; i++)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                Debug.Log($"{i} : {interactionUI} : valid {_view.IsValidSlot(interactionUI)}");
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            Debug.Log($"Before Check");
            for(int i = startIndex - 1; i >= 0; --i)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                Debug.Log($"{i} : {interactionUI} : valid {_view.IsValidSlot(interactionUI)}");
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            Debug.Log($"Cannot find next slot");
            
            return null;
        }

        #endregion Unity Methods

        #region InputSystem Callback
        
        public void OnInteractHandler()
        {

        }

        public void OnInteractScrollHandler(Vector2 scroll)
        {
            if(_view.SelectedSlot == null)
                return;
            int selectedIndex = _view.SelectedSlot.transform.GetSiblingIndex();
            selectedIndex += scroll.y > 0 ? -1 : 1;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, _view.SlotCount - 1);
            _view.SelectSlot(_view.GetSlotUIByIndex(selectedIndex));
        }     
        
        #endregion InputSystem Callback
    }
}

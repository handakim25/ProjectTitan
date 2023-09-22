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
            StartCoroutine(WaitRebuild());
        }

        private void OnDisable()
        {
            if(UIManager.Instance != null)
            {
                UIManager.Instance.OnInteractEvent -= OnInteractHandler;
                UIManager.Instance.OnInteractScrollEvent -= OnInteractScrollHandler;
            }
            _interactionList.OnInteractChanged -= InteractSlotChagned;
            _view.Clear();
        }

        IEnumerator WaitRebuild()
        {
            yield return new WaitForEndOfFrame();
            _view.UpdateGuideIcon();
            _view.SetCursorPos();
        }

        /// <summary>
        /// InteractionList부터 Callback을 받는 함수.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void InteractSlotChagned(Object sender, InteractionList.InteractSlotChangedEventArgs args)
        {
            // Bug
            // Selected Slot을 제거할 때 오류가 발생
            // 원인 : slot은 Update loop의 마지막에 삭제되기 때문에 child count를 루프의 마지막 값으로 설정해야 한다.
            if(args.RemovedObjects != null)
            {
                if(_view.SelectedSlot == null)
                {
                    Debug.LogError($"Can this happen?");
                    Debug.Log($"Slot Count : {_view.SlotCount} / Child Count : {_view.ChildCount}");
                    _view.SelectSlot(_view.GetSlotUIByIndex(0));
                    Debug.Log($"Selected Slot : {_view.SelectedSlot.name}");
                }
                int startIndex = _view.SelectedSlot.transform.GetSiblingIndex();
                _view.RemoveSlot(args.RemovedObjects);
                if(_view.SelectedSlot == null)
                {
                    _view.SelectSlot(FindNextSelect(startIndex));
                    if(_view.SelectedSlot == null && _view.SlotCount > 0)
                    {
                        Debug.LogError($"Failed to find next slot");
                    }
                }
                // Empty가 될 경우 Cursor Update하기 위함
                _view.SetCursorPos();
            }
            if(args.AddedObjects != null)
            {
                _view.AddSlot(args.AddedObjects);
            }
            _view.UpdateGuideIcon();

            if(_view.SelectedSlot == null && _view.SlotCount > 0)
            {
                // How this happen?
                Debug.LogError($"Can this happen? / Select First Slot");
                _view.SelectSlot(_view.GetSlotUIByIndex(0));
            }
        }

        /// <summary>
        /// startIndex를 제외한 Slot을 선택한다.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private GameObject FindNextSelect(int startIndex)
        {
            // Bug Fix
            // Object는 Update 이후에 삭제가 되기 때문에
            // 현재 Loop는 ChildCount만큼 순회를 해야한다.
            for(int i = 0; i < _view.ChildCount; ++i)
            {
                GameObject interacionUI = _view.GetSlotUIByIndex(i);
            }
            for(int i = startIndex + 1; i < _view.ChildCount; i++)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            for(int i = startIndex - 1; i >= 0; --i)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            
            return null;
        }

        #endregion Unity Methods

        #region InputSystem Callback
        
        /// <summary>
        /// Interact 버튼을 눌렀을 때 호출되는 함수
        /// </summary>
        public void OnInteractHandler()
        {
            if(_view.SelectedSlot == null)
            {
                Debug.Log($"Nothing selected");
                return;
            }

            var slot = _view.SelectedSlot;
            Debug.Log($"Do some interaction with {slot.name}");
            var go = slot.GetComponent<InteractionUI>().Interactable;
            if(go != null && go.TryGetComponent<Interactable>(out var interactable))
            {
                Debug.Log($"Interact with {interactable.name}");
                interactable.Interact();
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
    /// <summary>
    /// Interactio UI의 Controller. InteractionList로 부터 Interaction을 받아서 View에 표시한다.
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private InteractionList _interactionList;
        [SerializeField] private InteractionView _view;
        private bool _isInitialized = false;

        #region Unity Methods
        
        private void Start()
        {
            _isInitialized = true;
            _view.AddSlots(_interactionList.interactObjects.ToArray());
        }

        private void OnEnable()
        {
            UIManager.Instance.OnInteractEvent += OnInteractHandler;
            UIManager.Instance.OnInteractScrollEvent += OnInteractScrollHandler;
            _interactionList.OnInteractChanged += InteractSlotChagned;

            // 현재 Interaction List에 있는 Interaction을 View에 추가한다.
            // @Fix
            // Interaction List의 Awake 호출을 기다린다.
            // 첫 프레임에서는 Awake 단계에서 추가하는 것이 아니고 Start에서 추가한다.
            // OnEnable/OnDisable은 호출 순서에 주의할 것
            if(!_isInitialized)
            {
                return;
            }
            _view.AddSlots(_interactionList.interactObjects.ToArray());
        }

        private void OnDisable()
        {
            // 종료되는 시점에는 UIManager가 존재하지 않을 수 있다.
            if(UIManager.Instance != null)
            {
                UIManager.Instance.OnInteractEvent -= OnInteractHandler;
                UIManager.Instance.OnInteractScrollEvent -= OnInteractScrollHandler;
            }
            _interactionList.OnInteractChanged -= InteractSlotChagned;
            _view.Clear();
        }

        #endregion Unity Methods

        // 다른 UI 요소들이 업데이트 되는 것을 기다린다.
        // Cursor Pos의 경우 Force Update를 이용하기 때문에 대부분의 경우 업데이트가 정상적으로 이루어진다.
        // 하지만 OnEnable에서 
        IEnumerator WaitRebuild()
        {
            yield return new WaitForEndOfFrame();
            _view.UpdateGuideIcon();
            _view.UpdateCursorPos();
        }

        /// <summary>
        /// Interaction List의 Slot이 변경되었을 때 호출되는 콜백 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void InteractSlotChagned(Object sender, InteractionList.InteractSlotChangedEventArgs args)
        {
            // Bug
            // Selected Slot을 제거할 때 오류가 발생
            // 원인 : slot은 프레임 마지막에 삭제되기 때문에 child count를 루프의 마지막 값으로 설정해야 한다.
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
                _view.UpdateCursorPos();
            }
            if(args.AddedObjects != null)
            {
                _view.AddSlots(args.AddedObjects);
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
        /// startIndex를 제외한 Slot을 선택한다. start index 다음을 우선으로 찾고 없을 경우 start index의 역순으로 찾는다.
        /// </summary>
        /// <param name="startIndex">찾기 시작할 index, 해당 index를 제외하고 찾는다.</param>
        /// <returns>찾지 못햇을 경우 null</returns>
        private GameObject FindNextSelect(int startIndex)
        {
            if(startIndex < 0 || startIndex >= _view.ChildCount)
            {
                Debug.LogError($"Invalid Start Index : {startIndex}");
                return null;
            }

            // Bug Fix
            // Object는 Update 이후에 삭제가 되기 때문에
            // 현재 Loop는 interaction 개수가 아니라 ChildCount만큼 순회를 해야한다.

            // 원래 slot 다음부터 찾는다.
            for(int i = startIndex + 1; i < _view.ChildCount; i++)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            // 찾지 못했을 경우 역순으로 찾는다.
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

        #region InputSystem Callback
        
        /// <summary>
        /// Interact 버튼을 눌렀을 때 호출되는 함수
        /// </summary>
        public void OnInteractHandler()
        {
            if(_view.SelectedInteractable == null)
            {
                Debug.Log($"Nothing selected");
                return;
            }

            _view.SelectedInteractable.Interact();
        }

        // 마우스 휠 스크롤에 대한 콜백 함수
        public void OnInteractScrollHandler(Vector2 scroll)
        {
            if(_view.SelectedSlot == null)
                return;
            int selectedIndex = _view.SelectedSlot.transform.GetSiblingIndex();
            // 제일 상단이 0으로 시작하므로 Scroll이 위로 올라갈 경우 -1, 아래로 내려갈 경우 1을 더한다.
            selectedIndex += scroll.y > 0 ? -1 : 1;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, _view.SlotCount - 1);
            _view.SelectSlot(_view.GetSlotUIByIndex(selectedIndex));
        }     
        
        #endregion InputSystem Callback
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
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

        void InteractSlotChagned(Object sender, InteractionList.InteractSlotChangedEventArgs args)
        {
            if(args.RemovedObjects != null)
            {
                int startIndex = _view.SelectedSlot.transform.GetSiblingIndex();
                _view.RemoveSlot(args.RemovedObjects);
                if(_view.SelectedSlot == null)
                {
                    _view.SelectSlot(FindNextSelect(startIndex));
                }
            }
            if(args.AddedObjects != null)
            {
                _view.AddSlot(args.AddedObjects);
            }
        }

        private GameObject FindNextSelect(int startIndex)
        {
            for(int i = startIndex + 1; i < _view.SlotCount; i++)
            {
                GameObject interactionUI = _view.GetSlotUIByIndex(i);
                if(_view.IsValidSlot(interactionUI))
                {
                    return interactionUI;
                }
            }
            for(int i = startIndex - 1; i >= 0; ++i)
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
        
        public void OnInteractHandler()
        {

        }

        public void OnInteractScrollHandler(Vector2 scroll)
        {
            
        }     
        
        #endregion InputSystem Callback
    }
}

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
            _interactionList.OnInteractChanged += InteractSlotChagned;
        }

        private void OnDisable()
        {
            _interactionList.OnInteractChanged -= InteractSlotChagned;
        }

        void InteractSlotChagned(Object sender, InteractionList.InteractSlotChangedEventArgs args)
        {
            if(args.RemovedObjects != null)
            {
                _view.RemoveSlot(args.RemovedObjects);
            }
            if(args.AddedObjects != null)
            {
                _view.AddSlot(args.AddedObjects);
            }
        }
        
        #endregion Unity Methods
    }
}

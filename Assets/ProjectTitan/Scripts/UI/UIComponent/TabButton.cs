using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Titan.UI
{
    [RequireComponent(typeof(Image))]   
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public TabGroup tabGroup;

        public Image background;

        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeslected;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            background = GetComponent<Image>();
            tabGroup?.Subscribe(this);
        }

        #region EventSystem Callback
        
        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }
        
        #endregion EventSystem Callback        

        #region Methods
        
        public void Select()
        {
            OnTabSelected?.Invoke();
        }

        public void Deselect()
        {
            OnTabDeslected?.Invoke();
        }

        #endregion Methods
    }
}

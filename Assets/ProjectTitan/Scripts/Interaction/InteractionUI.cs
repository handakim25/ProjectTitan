using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
    /// <summary>
    /// Interaction Slot UI Controller
    /// </summary>
    [ExecuteAlways]
    public class InteractionUI : MonoBehaviour
    {
        /// <summary>
        /// InteractonUI가 가리키고 있는 Interactable Object
        /// GameObject로 가리키고 있는 것은 Interact 가능 오브젝트들이 입력으로 오기 때문
        /// </summary>
        public Interactable Interactable {get; set;}
        [field : SerializeField] public bool IsSelect {get; private set;} = false;

        public void Select()
        {
            IsSelect = true;
            OnSelect?.Invoke();
        }

        public void Deselect()
        {
            IsSelect = false;
            OnDeselect?.Invoke();
        }

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(IsSelect)
            {
                OnSelect?.Invoke();
            }
            else
            {
                OnDeselect?.Invoke();
            }
        }
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [field : SerializeField] public bool CanInteract {
            get;
            set;
        }
        [Range(0, 5)]
        [SerializeField] private float _interactRange = 3f;
        public float InteractRange => _interactRange;

        virtual public string InteractText => name;

        protected GameObject target;
        [Header("Event")]
        public UnityEvent OnInteract;
        public UnityEvent OnInteractIn;
        public UnityEvent OnInteractOut;

        public virtual void Interact()
        {
            OnInteract?.Invoke();
        }

        public virtual void OnInteractRangeIn()
        {
            OnInteractIn?.Invoke();
        }

        public virtual void OnInteractRangeOut()
        {
            OnInteractOut?.Invoke();
        }
    }
}

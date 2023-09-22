using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Interaction
{
    public class Interactable : MonoBehaviour
    {
        [Range(0, 5)]
        [SerializeField] private float _interactRange = 3f;

        public float InteractRange => _interactRange;
        [field : SerializeField] public bool CanInteract {get; protected set;} = true;
        virtual public string InteractText => name;

        protected GameObject target;

        public virtual void Interact()
        {

        }

        public virtual void OnInteractRangeIn()
        {

        }

        public virtual void OnInteractRangeOut()
        {

        }
    }
}

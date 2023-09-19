using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.DialogueSystem;

namespace Titan.Interaction
{
    public class DialogInteractable : Interactable
    {
        [SerializeField] private DialogueObject _dialogueObject;
        [SerializeField] private ReferenceID<DialogueObject> _dialogueObjectRef;

        public override void Interact()
        {
            if(_dialogueObject != null)
            {
                DialogueManager.Instance.StartDialogue(_dialogueObject);
            }
        }
    }
}

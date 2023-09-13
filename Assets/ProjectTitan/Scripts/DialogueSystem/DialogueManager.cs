using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using UnityEngine;

using Titan.UI;

namespace Titan.DialogueSystem
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [SerializeField] private DialogueUIController _dialogueUI;
        [Tooltip("다이얼로그가 종료되면 다음 다이얼로그가 바로 켜지지 않도록 텀을 둔다.")]
        [SerializeField] private float _dialogueInterval = 0.5f;

        // @After-Work
        private Dictionary<string, DialogueObject> _dialogueObjectDic = new();

        private DialogueObject _currentDialogueObject;
        private DialogueNode _currentDialogueNode;
        private float _lastDialogueEndTime;

        public void StartDialogue(string DialogueID)
        {
            if(_dialogueObjectDic.TryGetValue(DialogueID, out var dialogueObject))
            {
                StartDialogue(dialogueObject);
            }
            else
            {
                Debug.LogError("Dialogue ID : {DialogueID} is not exist.}");
            }
        }

        public void StartDialogue(DialogueObject dialogueObject)
        {
            if(Time.time - _lastDialogueEndTime < _dialogueInterval)
            {
                return;
            }
            if(_dialogueUI == null)
            {
                FindUI();
                if(_dialogueUI == null)
                {
                    Debug.LogError("Unable to find UI");
                    return;
                }
            }
            _currentDialogueObject = dialogueObject;
            _currentDialogueNode = dialogueObject.GetStartingNode();
            if(_currentDialogueNode == null)
            {
                Debug.LogError("Dialogue Object is empty");
                return;
            }

            _dialogueUI.GetComponent<DialogueUIScene>().OpenUI();
            _dialogueUI.SetDialogue(_currentDialogueNode.SpeakerName, _currentDialogueNode.DialogueText);
        }

        private DialogueNode GetNextDialogue()
        {
            if(_currentDialogueNode.NextNode != null)
            {
                return _currentDialogueObject.GetNode(_currentDialogueNode.NextNode);
            }

            return null;
        }

        private void EndDialogue()
        {
            _dialogueUI.GetComponent<DialogueUIScene>().CloseUI();

            _currentDialogueObject = null;
            _lastDialogueEndTime = Time.time;
        }

        private void FindUI()
        {
            _dialogueUI = FindObjectOfType<DialogueUIController>(true);
            if(_dialogueUI != null)
            {
                _dialogueUI.OnNextDialogue += OnNextDialogueHandler;
                _dialogueUI.OnDialougeEnd += OnDialogueEndHandler;
                _dialogueUI.OnChoiceSelected += OnChoiceSelectedHandler;
            }
        }

        #region Callbacks
        
        private void OnNextDialogueHandler()
        {
           if (_currentDialogueNode.choices.Count > 0)
            {
                return;
            } 
            // show next dialogue
            var nextNode = GetNextDialogue();
            ProcessDialogue(nextNode);
        }

        private void OnDialogueEndHandler()
        {
            if (_currentDialogueNode.choices.Count > 0)
            {
                // show choices
                var choiceText = _currentDialogueNode.choices.Select(x => x.ChoiceText).ToList();
                _dialogueUI.ShowChoice(choiceText);
            }
        }

        private void OnChoiceSelectedHandler(string choice)
        {
            // show next dialogue
            var nextNodeID = _currentDialogueNode.choices.Find(x => x.ChoiceText == choice).NextNode;
            var nextNode = _currentDialogueObject.GetNode(nextNodeID);
            ProcessDialogue(nextNode);
        }

        private void ProcessDialogue(DialogueNode node)
        {
            _currentDialogueNode = node;
            if(_currentDialogueNode != null)
            {
                if(!string.IsNullOrEmpty(_currentDialogueNode.TriggerEventID))
                {
                    // Trigger Event

                }
                _dialogueUI.SetDialogue(_currentDialogueNode.SpeakerName, _currentDialogueNode.DialogueText);
            }
            else
            {
                EndDialogue();
            }
        }
        
        #endregion Callback
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using UnityEngine;

using Titan.UI;
using Titan.QuestSystem;

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
        private ConditionEvaluator _conditionEvaluator;

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
           if (_currentDialogueNode.Choices.Count > 0)
            {
                return;
            } 
            // show next dialogue
            var nextNode = GetNextDialogue();
            ProcessDialogue(nextNode);
        }

        /// <summary>
        /// 선택지가 있으면 선택지를 출력한다.
        /// </summary>
        private void OnDialogueEndHandler()
        {
            if (_currentDialogueNode.Choices.Count > 0)
            {
                // show choices
                _conditionEvaluator ??= new ConditionEvaluator() {QuestManager = QuestManager.Instance};
                var choiceText = _currentDialogueNode.Choices.Where(x => x.Condition.IsMet(_conditionEvaluator)).Select(x => x.ChoiceText).ToList();
                // var choiceText = _currentDialogueNode.Choices.Select(x => x.ChoiceText).ToList();
                _dialogueUI.ShowChoice(choiceText);
            }
        }

        private void OnChoiceSelectedHandler(string choice)
        {
            // show next dialogue
            var nextNodeID = _currentDialogueNode.Choices.Find(x => x.ChoiceText == choice).NextNode;
            var nextNode = _currentDialogueObject.GetNode(nextNodeID);
            ProcessDialogue(nextNode);
        }

        /// <summary>
        /// 다음 대화를 처리한다. 만약 트리거를 발생해야 한다면 발생한다. 남은 대화가 없으면 종료한다.
        /// </summary>
        /// <param name="node"></param>
        private void ProcessDialogue(DialogueNode node)
        {
            _currentDialogueNode = node;
            if(_currentDialogueNode != null)
            {
                if(!string.IsNullOrEmpty(_currentDialogueNode.TriggerEventID))
                {
                    // Trigger Event

                }
                if(!string.IsNullOrEmpty(_currentDialogueNode.TriggerQuest))
                {
                    // Trigger Quest
                    EventBus.RaiseEvent(new QuestEvent
                    {
                        QuestID = _currentDialogueNode.TriggerQuest,
                        Status = System.Enum.Parse<QuestStatus>(_currentDialogueNode.TriggerQuestState),
                    });
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

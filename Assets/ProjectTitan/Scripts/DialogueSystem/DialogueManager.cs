using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using UnityEngine;

using Titan.UI;
using Titan.QuestSystem;
using Titan.Interaction;
using Titan.InventorySystem.Items;

namespace Titan.DialogueSystem
{
    /// <summary>
    /// Dialogue를 관리하는 Class. Dialogue를 시작하고 종료한다.
    /// Dialogue를 출력하는 것은 Dialogue UI가 담당한다.
    /// Dialogue UI 가 Controller이므로 UI가 Dialogue Manager를 관리하고
    /// Dialogue Manager가 반응해서 UI를 업데이트한다.
    /// </summary>
    sealed public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [SerializeField] private DialogueUIController _dialogueUI;
        [Tooltip("다이얼로그가 종료되면 다음 다이얼로그가 바로 켜지지 않도록 텀을 둔다.")]
        [SerializeField] private float _dialogueInterval = 0.5f;
        private float _lastDialogueEndTime;

        // @After-Work
        // 대화 데이터를 한 번에 모아서 JSON File로 저장하도록 구현하자.
        // 현재는 각각의 대화 데이터를 ScriptableObject로 저장하고 있다.
        private Dictionary<string, DialogueObject> _dialogueObjectDic = new();

        private DialogueObject _currentDialogueObject;
        private DialogInteractable _curDialogueInteractable;
        private DialogueNode _currentDialogueNode;
        private System.Action _onDialogueEnd;

        private string _curSpeaker => _curDialogueInteractable != null ? 
            _curDialogueInteractable.InteractText : null ?? _currentDialogueNode.SpeakerName;
        
        private ConditionEvaluator _conditionEvaluator;

        private DialogueUIController DialogueUI
        {
            get => _dialogueUI;
            set
            {
                if(_dialogueUI != null)
                {
                    _dialogueUI.OnNextDialogue -= OnNextDialogueHandler;
                    _dialogueUI.OnDialougeEnd -= OnDialogueEndHandler;
                    _dialogueUI.OnChoiceSelected -= OnChoiceSelectedHandler;
                }
                _dialogueUI = value;
                if(_dialogueUI != null)
                {
                    _dialogueUI.OnNextDialogue += OnNextDialogueHandler;
                    _dialogueUI.OnDialougeEnd += OnDialogueEndHandler;
                    _dialogueUI.OnChoiceSelected += OnChoiceSelectedHandler;
                }
            }
        }

        private void Start() 
        {
            if(_dialogueUI == null)
            {
                DialogueUI = FindObjectOfType<DialogueUIController>(true);
            }
        }

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

        public void StartDialogue(DialogueObject dialogueObject, DialogInteractable interactable = null, System.Action OnDialogueEnd = null)
        {
            // 너무 짧은 시간 동안 대화를 다시 실행하지 않도록 한다.
            if(Time.time - _lastDialogueEndTime < _dialogueInterval)
            {
                return;
            }
            if(_dialogueUI == null)
            {
                Debug.LogError("Dialogue UI is not exist");
                return;
            }
            _curDialogueInteractable = interactable;
            _onDialogueEnd = OnDialogueEnd;

            _currentDialogueObject = dialogueObject;
            _currentDialogueNode = dialogueObject.GetStartingNode();
            if(_currentDialogueNode == null)
            {
                Debug.LogError("Dialogue Object is empty");
                return;
            }

            _dialogueUI.GetComponent<DialogueUIScene>().OpenUI();
            _dialogueUI.SetDialogue(_curSpeaker, _currentDialogueNode.SentenceText);
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
            _onDialogueEnd?.Invoke();
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
                _conditionEvaluator ??= new ConditionEvaluator() {
                    QuestManager = QuestManager.Instance,
                    InventoryManager = InventoryManager.Instance,
                };
                var choiceTextList = _currentDialogueNode.Choices.Where(x => x.Condition.IsMet(_conditionEvaluator)).Select(x => x.ChoiceText).ToList();
                _dialogueUI.ShowChoice(choiceTextList);
            }
        }

        /// <summary>
        /// 선택지를 선택했을 때 처리한다.
        /// </summary>
        /// <param name="choiceStr">선택지 내용</param>
        private void OnChoiceSelectedHandler(string choiceStr)
        {
            // show next dialogue
            var choiceNode = _currentDialogueNode.Choices.Find(x => x.ChoiceText == choiceStr);

            var nextNodeID = choiceNode.NextNode;
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
                _dialogueUI.SetDialogue(_curSpeaker, _currentDialogueNode.SentenceText);
            }
            else
            {
                EndDialogue();
            }
        }
        
        #endregion Callback
    }
}

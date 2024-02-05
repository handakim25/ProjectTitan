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
        /// <summary>
        /// 마지막 대화가 끝난 시간(Time.time)
        /// </summary>
        private float _lastDialogueEndTime;

        // @After-Work
        // 대화 데이터를 한 번에 모아서 JSON File로 저장하도록 구현하자.
        // 현재는 각각의 대화 데이터를 ScriptableObject로 저장하고 있다.
        private Dictionary<string, DialogueObject> _dialogueObjectDic = new();

        private DialogueObject _currentDialogueObject;
        /// <summary>
        /// 현재 대화를 진행중인 대화 상대
        /// </summary>
        private DialogInteractable _curDialogueInteractable;
        private DialogueNode _curDialogueNode;
        /// <summary>
        /// Dialogue 종료 시에 호출되는 Callback
        /// </summary>
        private System.Action _onDialogueEnd;

        private string CurSpeaker => _curDialogueInteractable != null ? 
            _curDialogueInteractable.InteractText : null ?? _curDialogueNode.SpeakerName;
        
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
            if(DialogueUI == null)
            {
                DialogueUI = FindObjectOfType<DialogueUIController>(true);
            }
        }

        public void StartDialogue(string DialogueID, DialogInteractable interactable = null, System.Action OnDialogueEnd = null)
        {
            if(_dialogueObjectDic.TryGetValue(DialogueID, out var dialogueObject))
            {
                StartDialogue(dialogueObject, interactable, OnDialogueEnd);
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
            if(DialogueUI == null)
            {
                Debug.LogError("Dialogue UI is not exist");
                return;
            }
            _curDialogueInteractable = interactable;
            _onDialogueEnd = OnDialogueEnd;

            _currentDialogueObject = dialogueObject;
            _curDialogueNode = dialogueObject.GetStartingNode();
            if(_curDialogueNode == null)
            {
                Debug.LogError("Dialogue Object is empty");
                return;
            }

            DialogueUI.GetComponent<DialogueUIScene>().OpenUI();
            DialogueUI.SetDialogue(CurSpeaker, _curDialogueNode.SentenceText);
        }

        /// <summary>
        /// 다음 대화를 가져온다.
        /// </summary>
        /// <returns>대화를 다 가져왔으면 null을 반환</returns>
        private DialogueNode GetNextDialogue()
        {
            if(_curDialogueNode.NextNode != null)
            {
                return _currentDialogueObject.GetNode(_curDialogueNode.NextNode);
            }

            return null;
        }

        /// <summary>
        /// 대화를 종료한다. UI를 종료하고 대화가 끝났음을 알린다.
        /// </summary>
        private void EndDialogue()
        {
            DialogueUI.GetComponent<DialogueUIScene>().CloseUI();

            _currentDialogueObject = null;
            _lastDialogueEndTime = Time.time;
            _onDialogueEnd?.Invoke();
        }

        #region Callbacks
        
        /// <summary>
        /// 다음 대화를 출력한다.
        /// </summary>
        private void OnNextDialogueHandler()
        {
            // 선택지가 존재하면 다음 대화를 출력하지 않는다.
            // 선택지가 있다면 선택지를 선택해야된다.
           if (HasChoices)
            {
                return;
            } 

            var nextNode = GetNextDialogue();
            ProcessDialogue(nextNode);
        }

        /// <summary>
        /// 하나의 대화가 전부 출력됬을 때 호출되는 콜백. 선택지가 있으면 선택지를 출력한다.
        /// </summary>
        private void OnDialogueEndHandler()
        {
            if (HasChoices)
            {
                // show choices
                _conditionEvaluator ??= new ConditionEvaluator() {
                    QuestManager = QuestManager.Instance,
                    InventoryManager = InventoryManager.Instance,
                };
                var choiceTextList = _curDialogueNode.Choices.Where(x => x.Condition.IsMet(_conditionEvaluator))
                    .Select(x => x.ChoiceText)
                    .ToList();
                DialogueUI.ShowChoice(choiceTextList);
            }
        }

        private bool HasChoices => _curDialogueNode.Choices.Count > 0;

        /// <summary>
        /// 선택지를 선택했을 때 처리한다.
        /// </summary>
        /// <param name="choiceStr">선택지 내용</param>
        private void OnChoiceSelectedHandler(string choiceStr)
        {
            // show next dialogue
            var choiceNode = _curDialogueNode.Choices.Find(x => x.ChoiceText == choiceStr);
            if(choiceNode == null)
            {
                Debug.LogError("Choice is not exist");
                EndDialogue();
                return;
            }

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
            _curDialogueNode = node;
            if(_curDialogueNode != null)
            {
                if(!string.IsNullOrEmpty(_curDialogueNode.TriggerEventID))
                {
                    // Trigger Game Event
                    EventBus.RaiseEvent(new GameEventTriggeredEvent
                    {
                        EventName = _curDialogueNode.TriggerEventID,
                        TriggerStatus = _curDialogueNode.TriggerSetValue,
                    });
                }
                if(!string.IsNullOrEmpty(_curDialogueNode.TriggerQuest))
                {
                    // Trigger Quest
                    EventBus.RaiseEvent(new QuestEvent
                    {
                        QuestID = _curDialogueNode.TriggerQuest,
                        Status = System.Enum.Parse<QuestStatus>(_curDialogueNode.TriggerQuestState),
                    });
                }

                DialogueUI.SetDialogue(CurSpeaker, _curDialogueNode.SentenceText);
            }
            else
            {
                EndDialogue();
            }
        }
        
        #endregion Callback
    }
}

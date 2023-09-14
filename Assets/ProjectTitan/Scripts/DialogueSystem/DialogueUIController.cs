using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

using Titan.Core;
using System;
using UnityEngine.EventSystems;

namespace Titan.DialogueSystem
{
    public class DialogueUIController : MonoBehaviour, MainAction.IDialogueActions
    {
        [Header("UI Action")]
        [SerializeField] private InputActionAsset _inputAsset;
        private MainAction _action;

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _speakerText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private Button _autoButton;

        [Header("UI Settings")]
        [SerializeField] private float _dialogueTextSpeed = 0.1f;
        [Tooltip("Auto mode wait time")]
        [SerializeField] private float _autoWaitTime = 1f;
        [Tooltip("Block input interval when setntence starts")]
        [SerializeField] private float _blockInputInterval = 0.5f;
        private float _lastDialogueTextTime;

        [Header("UI Sound")]
        [SerializeField] private AudioClip _dialogueStartSound;
        [SerializeField] private AudioClip _dialogueEndSound;

        private string _curSentence;
        private bool _isAutoMode = false;
        private Coroutine _dialogueTextCoroutine;
        [HideInInspector] public bool _isDialogueAnimating = false;

        public event Action OnNextDialogue;
        public event Action OnDialougeEnd;
        public event Action<string> OnChoiceSelected;

        private void Awake()
        {
            Debug.Assert(_speakerText != null, "Name Text is null");
            Debug.Assert(_dialogueText != null, "Dialogue Text is null");
            Debug.Assert(_inputAsset != null, "Input Asset is null");
            Debug.Assert(_choicePanel != null, "Choice Panel is null");
            Debug.Assert(_autoButton != null, "Auto Button is null");

            _action = new MainAction(_inputAsset);
            _action.Dialogue.SetCallbacks(this);
            _action.Dialogue.Disable();

            _autoButton.onClick.AddListener(ToggleAuto);
        }

        private void OnEnable()
        {
            _isDialogueAnimating = true;

            // Player Input은 UI Manager가 관리해주고 있다.
            _action.Dialogue.Enable();
            _action.UI.Disable();

            _isAutoMode = false;
            _autoButton.image.color = _isAutoMode ? Color.yellow : Color.white;
        }

        private void OnDisable()
        {
            _action.Dialogue.Disable();
            _action.UI.Enable();
        }

        public void SetDialogue(string name, string sentence)
        {
            if(_choicePanel.activeSelf)
            {
                if(_choicePanel.GetComponent<ScrollRect>().content.childCount > 0)
                {
                    foreach(Transform child in _choicePanel.GetComponent<ScrollRect>().content)
                    {
                        Destroy(child.gameObject);
                    }
                }
                _choicePanel.SetActive(false);
            }

            _lastDialogueTextTime = Time.time;
            _speakerText.text = name;
            _curSentence = sentence;
            if(_dialogueTextCoroutine != null)
            {
                StopCoroutine(_dialogueTextCoroutine);
                _dialogueTextCoroutine = null;
            }
            _dialogueTextCoroutine = StartCoroutine(ShowDialogueText(_curSentence));
        }

        // 중복된 입력에 대한 처리는 누구의 책임이지?
        // 일단 입력에 대한 처리는 UI Controller의 역할이라고 가정한다.
        public void ShowChoice(List<string> choices)
        {
            if(_choicePanel.activeSelf)
            {
                return;
            }

            _choicePanel.SetActive(true);
            
            var choiceContent = _choicePanel.GetComponent<ScrollRect>().content;
            foreach(var choice in choices)
            {
                var choiceButton = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DialogueChoiceButton"), choiceContent.transform);
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice;
                choiceButton.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected?.Invoke(choice));
            }
        }
        
        IEnumerator ShowDialogueText(string sentence)
        {
            _dialogueText.text = "";

            float waitLimit = Time.time + 3.0f;
            while(_isDialogueAnimating && Time.time < waitLimit)
            {
                yield return null;
            }

            // Show Dialogue text one by one
            for(int i = 0; i < sentence.Length; i++)
            {
                _dialogueText.text += sentence[i];
                yield return new WaitForSeconds(_dialogueTextSpeed);
            }

            OnDialougeEnd?.Invoke();
            
            if(_isAutoMode && _autoWaitTime > 0f)
            {
                Debug.Log($"Start Wait Auto");
                yield return new WaitForSeconds(_autoWaitTime);
                OnDialougeEnd?.Invoke();
                OnNextDialogue?.Invoke();
            }

            _dialogueTextCoroutine = null;
        }

        // Process Next Dialogue
        // 1. If Dialogue Text is animating, stop animating and show all text.
        // 2. If Choice exists, wait selection.
        // 3. If Choice does not exist, show next dialogue.

        /// <summary>
        /// <para> - Input Callback </para>
        /// <para> - Auto Next Dialogue </para>
        /// </summary>
        public void ProcessNextDialogue()
        {
            if(Time.time - _lastDialogueTextTime < _blockInputInterval)
            {
                return;
            }
            if(_dialogueTextCoroutine != null)
            {
                StopCoroutine(_dialogueTextCoroutine);
                _dialogueTextCoroutine = null;
                _dialogueText.text = _curSentence;
                OnDialougeEnd?.Invoke();
            }
            else
            {
                // If Choice exists, wait selection.
                if(!_choicePanel.activeSelf)
                {
                    OnNextDialogue?.Invoke();
                }
            }
        }

        public void ToggleAuto()
        {
            _isAutoMode = !_isAutoMode;
            
            _autoButton.image.color = _isAutoMode ? Color.yellow : Color.white;

            if(_isAutoMode && _dialogueTextCoroutine == null)
            {
                ProcessNextDialogue();
            }
        }

        // Input Callback
        void MainAction.IDialogueActions.OnNextDialogue(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                // Memo
                // action.name : action, 즉 입력의 종류
                // control.name : 어떤 입력인지

                var device = InputSystem.GetDevice<Pointer>();
                if(device != null && IsRaycastHitTargetUI(device.position.ReadValue()))
                {
                    return;
                }
                ProcessNextDialogue();
            }
        }

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new();
        private bool IsRaycastHitTargetUI(Vector2 position)
        {
            _pointerEventData ??= new PointerEventData(EventSystem.current);
            _pointerEventData.position = position;
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            return _raycastResults.Any(result => result.gameObject == _autoButton.gameObject);
        }
    }
}

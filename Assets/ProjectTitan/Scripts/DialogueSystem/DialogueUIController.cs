using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

using Titan.Core;
using Titan.UI;

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
        [Tooltip("선택지 패널 Object")]
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private Button _autoButton;

        [Header("UI Settings")]
        [Tooltip("문자간 출력 간격")]
        [SerializeField] private float _dialogueTextSpeed = 0.1f;
        [Tooltip("Auto 모드일 때 텍스트가 출력된 후 대기 시간")]
        [SerializeField] private float _autoWaitTime = 1f;
        [Tooltip("텍스트가 출력된 후 입력을 차단하는 시간")]
        [SerializeField] private float _blockInputInterval = 0.5f;
        private float _lastDialogueTextTime;

        [Header("UI Sound")]
        [Tooltip("대화창이 열릴 때 나오는 사운드")]
        [SerializeField] private SoundList _dialogueStartSound = SoundList.None;
        [Tooltip("대화 종료 시 재생되는 사운드")]
        [SerializeField] private SoundList _dialogueEndSound = SoundList.None;

        /// <summary>
        /// 현재 출력중인 대사
        /// </summary>
        private string _curSentence;
        private bool _isAutoMode = false;
        /// <summary>
        /// 대사 출력 코루틴. 스킵 등이 발생하면 코루틴을 중단하고 출력을 마무리한다.
        /// </summary>
        private Coroutine _dialogueTextCoroutine;
        /// <summary>
        /// Dialgoue Animation이 진행중인지를 나타내는 변수. 애니메이션이 종료되고 내용이 출력된다.
        /// </summary>
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
                yield return new WaitForSecondsRealtime(_dialogueTextSpeed);
            }

            OnDialougeEnd?.Invoke();
            
            if(_isAutoMode && _autoWaitTime > 0f)
            {
                Debug.Log($"Start Wait Auto");
                yield return new WaitForSecondsRealtime(_autoWaitTime);
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
                if(device != null && UIManager.Instance.IsRaycastHitTargetUI(device.position.ReadValue()))
                {
                    return;
                }
                ProcessNextDialogue();
            }
        }
    }
}

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
        [UnityEngine.Serialization.FormerlySerializedAs("_dialogueText")]
        [SerializeField] private TextMeshProUGUI _sentenceText;
        [Tooltip("선택지 패널 Object")]
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private Button _autoButton;

        [Header("Sentence Settings")]
        [Tooltip("문자간 출력 간격")]
        [SerializeField] private float _charOutputInterval = 0.1f;
        [Tooltip("Auto 모드일 때 텍스트가 출력된 후 대기 시간")]
        [SerializeField] private float _autoWaitTime = 1f;
        [Tooltip("텍스트가 출력된 후 입력을 차단하는 시간")]
        [SerializeField] private float _blockInputDuration = 0.5f;
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

        /// <summary>
        /// 다음 대사가 출력할 때 발생하는 이벤트
        /// </summary>
        public event Action OnNextDialogue;
        /// <summary>
        /// 모든 대사가 출력됬을 때 발생하는 이벤트
        /// </summary>
        public event Action OnDialougeEnd;
        /// <summary>
        /// Choice가 선택되었을 때 발생하는 이벤트
        /// </summary>
        public event Action<string> OnChoiceSelected;

        private void Awake()
        {
            Debug.Assert(_speakerText != null, "Name Text is null");
            Debug.Assert(_sentenceText != null, "Dialogue Text is null");
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

            // @Note
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

        /// <summary>
        /// 대화창을 시작한다.
        /// </summary>
        /// <param name="name">화자 이름</param>
        /// <param name="sentence">대사</param>
        public void SetDialogue(string name, string sentence)
        {
            // @refactor
            // Object Pooling으로 교체할 것
            // Choice Panel 초기화
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

            // 대사를 출력하는 코루틴이 실행중이면 중단한다.
            if(_dialogueTextCoroutine != null)
            {
                StopCoroutine(_dialogueTextCoroutine);
                _dialogueTextCoroutine = null;
            }

            _dialogueTextCoroutine = StartCoroutine(ShowDialogueText(_curSentence));
        }

        // @After Work
        // 선택지 검증 코드 추가할 것
        /// <summary>
        /// 선택지를 출력한다.
        /// </summary>
        /// <param name="choices">선택지 내용</param>
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
        
        /// <summary>
        /// 선택지를 출력하는 코루틴.
        /// 출력만을 담당하며 UI 입력은 콜백에서 처리한다.
        /// </summary>
        /// <param name="sentence">대사</param>
        IEnumerator ShowDialogueText(string sentence)
        {
            _sentenceText.text = "";

            // 최대 3초간 대기, 문제가 생겨서 3초가 넘었을 경우 강제로 출력
            float waitLimit = Time.time + 3.0f;
            while(_isDialogueAnimating && Time.time < waitLimit)
            {
                yield return null;
            }

            // 대사를 출력
            for(int i = 0; i < sentence.Length; i++)
            {
                _sentenceText.text += sentence[i];
                yield return new WaitForSecondsRealtime(_charOutputInterval);
            }

            OnDialougeEnd?.Invoke();
            
            if(_isAutoMode && _autoWaitTime > 0f)
            {
                Debug.Log($"Start Wait Auto");
                yield return new WaitForSecondsRealtime(_autoWaitTime);
                // @Fix
                // 다이얼로그가 끝나고 기본적으로 선택지가 출력이 되기 때문에 중복으로 호출할 필요가 없다.
                // OnDialougeEnd?.Invoke();
                OnNextDialogue?.Invoke();
            }

            _dialogueTextCoroutine = null;
        }

        // Process Next Dialogue
        // 1. 다이얼로그가 진행 중이면, 진행을 중단하고 모든 텍스트를 출력한다.
        // 2. 선택지가 있으면 선택을 기다린다.
        // 3. 선택지가 없으면 다음 대사를 출력한다.

        /// <summary>
        /// <para> 호출 시점 </para>
        /// <para> - Input Callback </para>
        /// <para> - Auto Next Dialogue </para>
        /// </summary>
        /// <remarks> 입력을 차단하는 함수가 있기 때문에 항상 다음 문장이 출력되지 않는다. Auto mode에서 주의할 것 </remarks>
        public void ProcessNextDialogue()
        {
            if(Time.time - _lastDialogueTextTime < _blockInputDuration)
            {
                return;
            }
            if(_dialogueTextCoroutine != null)
            {
                StopCoroutine(_dialogueTextCoroutine);
                _dialogueTextCoroutine = null;
                _sentenceText.text = _curSentence;
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

            // Auto mode이고 대사가 전부 출력됬다면 자동으로 다음 대사를 출력한다.
            if(_isAutoMode && _dialogueTextCoroutine == null)
            {
                ProcessNextDialogue();
            }
        }

        #region Input Callback
        
        void MainAction.IDialogueActions.OnNextDialogue(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                // @Memo
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
        
        #endregion Input Callback
    }
}

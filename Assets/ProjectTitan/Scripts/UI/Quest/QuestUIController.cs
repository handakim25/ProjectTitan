using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Assertions;
using TMPro;

using Titan.QuestSystem;

namespace Titan.UI
{
    /// <summary>
    /// Quest UI를 실질적으로 그리는 클래스
    /// </summary>
    public class QuestUIController : MonoBehaviour
    {
        [Header("UI/퀘스트 선택")]
        [Tooltip("퀘스트 목록을 표시하는 ScrollRect")]
        [SerializeField] private ScrollRect _questListScrollRect;
        [FormerlySerializedAs("_questItemPrefab")]
        [SerializeField] private GameObject _qusetSelectPrefab;

        [Header("UI/퀘스트 상세 정보")]
        [Tooltip("퀘스트 제목을 표시")]
        [SerializeField] private TextMeshProUGUI _questTitleText;
        [Tooltip("퀘스트 목표를 표시")]
        [SerializeField] private TextMeshProUGUI _questObjectText;
        [SerializeField] private string _questObjectBulletStr = "•";
        
        [Tooltip("퀘스트 설명을 표시")]
        [SerializeField] private TextMeshProUGUI _questDescriptionText;

        [Space]
        [SerializeField] private GameObject _noQuestImage;

        /// <summary>
        /// 현재 표시되고 있는 퀘스트의 진행 상황 데이터
        /// </summary>
        private List<QuestProgressData> curAcceptedQuestList;
        private GameObject _selectedQuestButton;
        // Select로 넘어온 시점에 Button은 Select된 상태이다.
        private GameObject SelectedQuestButton
        {
            get => _selectedQuestButton;
            set
            {
                if(_selectedQuestButton == value)
                {
                    return;
                }
                if(_selectedQuestButton != null)
                {
                    _selectedQuestButton.GetComponent<TweenButton>().Deselect();
                }
                _selectedQuestButton = value;
            }
        }
        
        private void Awake()
        {
            Assert.IsNotNull(_questListScrollRect);
            Assert.IsNotNull(_qusetSelectPrefab);
        }

        private void OnEnable()
        {
            curAcceptedQuestList = QuestManager.Instance.GetAcceptedQuestList();            
            CreateQuestItems();
            if(_questListScrollRect.content.childCount > 0)
            {
                var firstGo  = _questListScrollRect.content.GetChild(0).gameObject;
                firstGo.GetComponent<TweenButton>().Select();
                SelectedQuestButton = firstGo; // Tab Button과도 유사하다. Group 기능을 추가하는 것도 고려할 법하다.
            }
            ShowNoQuestImage(curAcceptedQuestList.Count == 0);
            
            // @To-Do
            // 만약에 퀘스트 진행 사항을 클리어하는 부분이 있어서 갱신이 되어야 되는 상황이 있을 수 있다.
            // 그럴 경우 QuestManager를 Subscribe해서 갱신을 해줘야 한다.
            // 혹은 기존에 사용되고 있는 Game Event System을 Subscribe할 것
        }

        private void OnDisable()
        {
            curAcceptedQuestList = null;
            DestroySlots();
        }
        
        /// <summary>
        /// Quest 항목의 리스트를 생성
        /// </summary>
        private void CreateQuestItems()
        {
            foreach(var curQuestProgress in curAcceptedQuestList)
            {
                if(QuestManager.Instance.TryGetQuest(curQuestProgress.QuestID, out var quest))
                {
                    var questSelectButton = Instantiate(_qusetSelectPrefab, _questListScrollRect.content);
                    SetupQuestSelectButton(quest, questSelectButton);
                }
                else
                {
                    Debug.LogError($"No quest {curQuestProgress.QuestID}");
                }
            }
        }

        private void SetupQuestSelectButton(Quest quest, GameObject questSelectButton)
        {
            questSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = quest.QuestName;
            if(questSelectButton.TryGetComponent<TweenButton>(out var tweenButton))
            {
                tweenButton.OnButtonSelected.AddListener(() => OnQuestButtonClicked(questSelectButton, quest));
            }
            else
            {
                // Tween Button에서 Animation 정보가 가지고 있기 때문에 없을 때 Tween Button을 추가하는 식으로 해결하지 않는다.
                Debug.LogError($"No TweenButton Component in {questSelectButton.name}");
            }
        }

        private void DestroySlots()
        {
            foreach(Transform child in _questListScrollRect.content)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Quest 선택이 클릭됬을 때의 Callback 함수
        /// </summary>
        /// <param name="clickedButton"></param>
        /// <param name="questObject"></param>
        private void OnQuestButtonClicked(GameObject clickedButton, Quest questObject)
        {
            SelectedQuestButton = clickedButton;
            ShowQuestDetail(questObject);
        }

        /// <summary>
        /// 클릭됬을 때 Quest의 상세 정보를 보여줌
        /// </summary>
        /// <param name="questObject"></param>
        private void ShowQuestDetail(Quest questObject)
        {
            Debug.Log($"Show Quset Detail : {questObject.QuestName}");
            _questTitleText.text = questObject.QuestName;
            _questObjectText.gameObject.SetActive(questObject.QuestObjectDescription != null && questObject.QuestObjectDescription.Length > 0);
            var strWithBullet = questObject.QuestObjectDescription?
                .Select(s => string.IsNullOrEmpty(_questObjectBulletStr) ? s : $"{_questObjectBulletStr} {s}") ?? Enumerable.Empty<string>();
            _questObjectText.text = string.Join("\n", strWithBullet);
            _questDescriptionText.text = questObject.QuestDescription;
        }

        private void ShowNoQuestImage(bool isShow)
        {
            if(_noQuestImage ==  null)
            {
                return;
            }
            _noQuestImage.SetActive(isShow);
        }

#if UNITY_EDITOR
        [ContextMenu("Accept Test Quest")]
        private void AcceptTestQuest()
        {
            EventBus.RaiseEvent(new QuestEvent
            {
                QuestID = "Next_Quest",
                Status = QuestStatus.Received,
            });
        }
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;

using Titan.QuestSystem;

namespace Titan.UI
{
    /// <summary>
    /// Quest UI를 실질적으로 그리는 클래스
    /// </summary>
    public class QuestUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScrollRect _questListScrollRect;
        [FormerlySerializedAs("_questItemPrefab")]
        [SerializeField] private GameObject _qusetSelectPrefab;
        [Space]
        [Tooltip("퀘스트 제목을 표시")]
        [SerializeField] private TextMeshProUGUI _questTitleText;
        [Tooltip("퀘스트 목표를 표시")]
        [SerializeField] private TextMeshProUGUI _questObjectText;
        [Tooltip("퀘스트 설명을 표시")]
        [SerializeField] private TextMeshProUGUI _questDescriptionText;

        /// <summary>
        /// 현재 표시되고 있는 퀘스트의 진행 상황 데이터
        /// </summary>
        private List<QuestProgressData> curAcceptedQuestList;

        private void OnEnable()
        {
            curAcceptedQuestList = QuestManager.Instance.GetAcceptedQuestList();            
            CreateQuestItems();
            if(curAcceptedQuestList.Count > 0)
            {
                ShowQuestDetail(QuestManager.Instance.GetQuest(curAcceptedQuestList[0].QuestID));
            }
            else
            {
                _questTitleText.text = "No Quest";
                _questObjectText.text = "";
                _questDescriptionText.text = "";
            }

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
                tweenButton.OnButtonSelected.AddListener(() => ShowQuestDetail(quest));
            }
            else
            {
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
        /// 클릭됬을 때 Quest의 상세 정보를 보여줌
        /// </summary>
        /// <param name="questObject"></param>
        private void ShowQuestDetail(Quest questObject)
        {
            _questTitleText.text = questObject.QuestName;
            _questObjectText.text = "";
            foreach(var quest in questObject.QuestObjectDescription)
            {
                _questObjectText.text += quest + "\n";
            }
            _questDescriptionText.text = questObject.QuestDescription;
        }
    }
}

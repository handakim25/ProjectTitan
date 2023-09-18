using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Titan.QuestSystem;

namespace Titan.UI
{
    public class QuestUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScrollRect _questListScrollRect;
        [SerializeField] private GameObject _questItemPrefab;
        [Space]
        [SerializeField] private TextMeshProUGUI _questTitleText;
        [SerializeField] private TextMeshProUGUI _questObjectText;
        [SerializeField] private TextMeshProUGUI _questDescriptionText;

        private List<QuestProgressData> currentQuestProgressData;

        private void OnEnable()
        {
            currentQuestProgressData = QuestManager.Instance.GetAcceptedQuestList();            
            CreateQuestItems();
            if(currentQuestProgressData.Count > 0)
            {
                ShowQuestDetail(QuestManager.Instance.GetQuest(currentQuestProgressData[0].QuestID));
            }
            else
            {
                _questTitleText.text = "No Quest";
                _questObjectText.text = "";
                _questDescriptionText.text = "";
            }
        }

        private void OnDisable()
        {
            currentQuestProgressData = null;
            DestroySlots();
        }
        
        /// <summary>
        /// Quest 항목의 리스트를 생성
        /// </summary>
        private void CreateQuestItems()
        {
            foreach(var progress in currentQuestProgressData)
            {
                if(QuestManager.Instance.TryGetQuest(progress.QuestID, out var quest))
                {
                    var questItem = Instantiate(_questItemPrefab, _questListScrollRect.content);
                    SetupQuestItem(quest, questItem);
                }
                else
                {
                    Debug.LogError($"No quest {progress.QuestID}");
                }
            }
        }

        private void SetupQuestItem(Quest quest, GameObject questItem)
        {
            questItem.GetComponent<Button>().onClick.AddListener(() => ShowQuestDetail(quest));
            questItem.GetComponentInChildren<TextMeshProUGUI>().text = quest.QuestName;
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
            foreach(var quest in questObject.QuestObjectDescription)
            {
                _questObjectText.text += quest + "\n";
            }
            _questDescriptionText.text = questObject.QuestDescription;
        }
    }
}

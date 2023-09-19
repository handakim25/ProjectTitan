using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using System.Linq;
using System;

namespace Titan.QuestSystem
{
    public class QuestManager : MonoSingleton<QuestManager>
    {
        // 지금은 통합이지만 Received 만 남기고 completed는 따로 남기자.
        // 퀘스트 양이 엄청나게 많아지면 문제가 생길 수 있다.
        private Dictionary<string, QuestProgressData> questProgressDictionary = new();

        private void OnEnable()
        {
            EventBus.RegisterCallback<QuestEvent>(QuestEventHandler);
        }

        private void OnDisable()
        {
            EventBus.UnregisterCallback<QuestEvent>(QuestEventHandler);
        }

        public void ReceiveQuest(string questID, bool notify = false)
        {
            if(questProgressDictionary.ContainsKey(questID))
            {
                Debug.LogWarning($"Already received quest {questID}");
                return;
            }
            if(!DataManager.QuestDatabase.TryGetQuest(questID, out var quest))
            {
                Debug.LogWarning($"No quest {questID}");
                return;
            }

            var questProgressData = new QuestProgressData
            {
                QuestID = questID,
                Status = QuestStatus.Received,
                CurrentProgress = 0,
            };
            questProgressDictionary.Add(questID, questProgressData);

            if (notify)
            {
                var QuestEvent = new QuestEvent
                {
                    QuestID = questID,
                    Status = QuestStatus.Received,
                };
                EventBus.RaiseEvent(QuestEvent);
            }
        }

        public void CompleteQuest(string questID, bool notify = false)
        {
            if(!questProgressDictionary.ContainsKey(questID))
            {
                Debug.LogWarning($"No quest {questID}");
                return;
            }

            var questProgressData = questProgressDictionary[questID];
            questProgressData.Status = QuestStatus.Completed;

            if (notify)
            {
                var QuestEvent = new QuestEvent
                {
                    QuestID = questID,
                    Status = QuestStatus.Completed,
                };
                EventBus.RaiseEvent(QuestEvent);
            }
        }

        public List<QuestProgressData> GetAcceptedQuestList()
        {
            // Quest 량이 엄청나게 길어지면 이것은 유효하지 않게 된다.
            return questProgressDictionary.Values.Where(questProgressData => questProgressData.Status == QuestStatus.Received).ToList();
        }

        public Quest GetQuest(string questID)
        {
            return DataManager.QuestDatabase.GetQuest(questID);
        }

        public bool TryGetQuest(string questID, out Quest quest)
        {
            return DataManager.QuestDatabase.TryGetQuest(questID, out quest);
        }

        public QuestStatus GetQuestStatus(string QuestID)
        {
            if(!questProgressDictionary.ContainsKey(QuestID))
            {
                return QuestStatus.NotReceived;
            }
            return questProgressDictionary[QuestID].Status;
        }

        // private void ItemCollectHandler(ItemCollectedEvent itemCollectedEvent)
        // {
        //     foreach(var questProgressData in questProgressDictionary.Values)
        //     {
        //         var quest = DataManager.QuestDatabase.GetQuest(questProgressData.QuestID);
        //         if(quest == null)
        //         {
        //             Debug.LogWarning($"No quest {questProgressData.QuestID}");
        //             continue;
        //         }

        //         foreach(var questGoal in quest.QuestGoal)
        //         {
        //             if(questGoal.Type != QuestRequirement.RequirementType.Item)
        //             {
        //                 continue;
        //             }

        //             // Inventory System과 통신해서 처리
        //         }
        //     }
        // }

        private void EnemyDeadEventHandler(EnemyDeadEvent enemyDeadEvent)
        {
            foreach(var questProgressData in questProgressDictionary.Values)
            {
                var quest = DataManager.QuestDatabase.GetQuest(questProgressData.QuestID);
                if(quest == null)
                {
                    Debug.LogWarning($"No quest {questProgressData.QuestID}");
                    continue;
                }

                foreach(var questGoal in quest.QuestObjectRequirement)
                {
                    if(questGoal.Type != Requirement.RequirementType.MonsterKill)
                    {
                        continue;
                    }

                    if(questGoal.TargetID == enemyDeadEvent.EnemyID)
                    {
                        questProgressData.CurrentProgress++;
                        questProgressData.CurrentProgress = Mathf.Clamp(questProgressData.CurrentProgress, 0, questGoal.TargetCount);
                    }
                }
            }
        }

        // Some Save / Load Logic

        private void QuestEventHandler(QuestEvent questEvent)
        {
            switch (questEvent.Status)
            {
                case QuestStatus.Received:
                    ReceiveQuest(questEvent.QuestID);
                    break;
                case QuestStatus.Completed:
                    CompleteQuest(questEvent.QuestID);
                    break;
                default:
                    Debug.LogError("Invalid Quest Status");
                    break;
            }
        }
    }
}

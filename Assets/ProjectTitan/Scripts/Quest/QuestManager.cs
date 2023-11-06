using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using System.Linq;
using System;
using Titan.GameEventSystem;

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
            EventBus.RegisterCallback<EnemyDeadEvent>(EnemyDeadEventHandler);
        }

        private void OnDisable()
        {
            EventBus.UnregisterCallback<QuestEvent>(QuestEventHandler);
            EventBus.UnregisterCallback<EnemyDeadEvent>(EnemyDeadEventHandler);
        }

        /// <summary>
        /// 퀘스트를 받는다.
        /// </summary>
        /// <param name="questID">받을려는 QuestID</param>
        /// <param name="notify">true일 경우 GameEvent를 발생</param>
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

        /// <summary>
        /// Quest를 Complete 상태로 전환하고 처리한다.
        /// Reward를 주고, Event를 발생시킨다.
        /// </summary>
        /// <param name="questID">Quest ID</param>
        /// <param name="notify">true일 경우 이벤트 발생</param>
        public void CompleteQuest(string questID, bool notify = false)
        {
            if(!questProgressDictionary.ContainsKey(questID))
            {
                Debug.LogWarning($"No quest {questID}");
                return;
            }

            SetQuestComplete(questID, notify);

            var quest = GetQuest(questID);
            if(quest.QuestCompleteTriggerEvents != null)
            {
                foreach(var triggerEvent in quest.QuestCompleteTriggerEvents)
                {
                    GameEventManager.Instance.SetEventStatus(triggerEvent, true);
                }
            }
        }

        private void SetQuestComplete(string questID, bool notify = false)
        {
            var questProgressData = questProgressDictionary[questID];
            if(questProgressData.Status == QuestStatus.Completed)
            {
                Debug.LogWarning($"Already completed quest {questID}");
                return;
            }
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

        /// <summary>
        /// Accep 상태인 Quest를 반환한다.
        /// </summary>
        /// <returns></returns>
        public List<QuestProgressData> GetAcceptedQuestList()
        {
            // Quest 량이 엄청나게 길어지면 이것은 유효하지 않게 된다.
            return questProgressDictionary.Values.Where(questProgressData => questProgressData.Status == QuestStatus.Received).ToList();
        }

        /// <summary>
        /// QuestID를 기준으로 Quest를 반환한다.
        /// </summary>
        /// <param name="questID">해당하는 Quest가 없을 경우 null이 반환된다.</param>
        /// <returns></returns>
        public Quest GetQuest(string questID)
        {
            return DataManager.QuestDatabase.GetQuest(questID);
        }

        /// <summary>
        /// QuestID를 기준으로 Try 형식으로 퀘스트를 반환한다.
        /// </summary>
        /// <param name="questID">Quest ID</param>
        /// <param name="quest">Quest 반환값</param>
        /// <returns>true일 경우 존재, false일 경우 존재하지 않는다</returns>
        public bool TryGetQuest(string questID, out Quest quest)
        {
            return DataManager.QuestDatabase.TryGetQuest(questID, out quest);
        }

        /// <summary>
        /// QuestID를 기반으로 QuestStatus를 반환한다.
        /// </summary>
        /// <param name="QuestID">QuestID</param>
        /// <returns>Quest가 없을 경우 Not Received를 반환</returns>
        public QuestStatus GetQuestStatus(string QuestID)
        {
            if(!questProgressDictionary.ContainsKey(QuestID))
            {
                return QuestStatus.NotReceived;
            }
            return questProgressDictionary[QuestID].Status;
        }

        /// <summary>
        /// Enemy가 죽는 것을 확인하는 이벤트 핸들러
        /// </summary>
        /// <param name="enemyDeadEvent"></param>
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

        /// <summary>
        /// Quest Event를 처리하는 이벤트 핸들러
        /// </summary>
        /// <param name="questEvent"></param>
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Titan.QuestSystem
{
    /// <summary>
    /// Run-time에서 진행되는 Quest 데이터
    /// </summary>
    [System.Serializable]
    public class Quest
    {
        public int QuestIndex;
        public string QuestID;
        public string QuestName;
        [TextArea(3, 5)]
        public string QuestDescription;
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestType Type;
        /// <summary>
        /// Run-time에서 불러온다.
        /// </summary>
        [System.NonSerialized] public QuestStatus Status;
        public string[] QuestObjectDescription;
        public QuestRequirement[] QuestObjectRequirement;
        /// <summary>
        /// Quest를 받을 수 있는 조건
        /// </summary>
        public QuestRequirement[] QuestAcceptCondition;
        public int QuestRewardGold;
        public int QuestRewardExp;
        public List<string> QuestRewardItems = new();
        public List<string> QuestCompleteTriggerEvents = new();
    }

    public enum QuestStatus
    {
        NotReceived,
        Received,
        Completed,
        Failed,
    }

    public enum QuestType
    {
        Main,
        Sub,
    }
}

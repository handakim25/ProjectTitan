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
        [Tooltip("게임에서 표시되는 퀘스트의 이름")]
        public string QuestName;
        [TextArea(3, 5)]
        [Tooltip("퀘스트 창에서 표시되는 퀘스트 설명")]
        // Unity Json Converter를 이용할 경우 Type이 Readable하지 않으므로 Newtonsoft Json Converter를 사용한다.
        public string QuestDescription;
        [JsonConverter(typeof(StringEnumConverter))]
        public QuestType Type;
        /// <summary>
        /// Run-time에서 불러온다.
        /// </summary>
        [System.NonSerialized] public QuestStatus Status;
        [Tooltip("퀘스트 창에서 퀘스트 목적 설명")]
        public string[] QuestObjectDescription;
        [Tooltip("퀘스트 수행 조건")]
        public Requirement[] QuestObjectRequirement;
        [Tooltip("퀘스트 수락 조건")]
        public Requirement[] QuestAcceptCondition;
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

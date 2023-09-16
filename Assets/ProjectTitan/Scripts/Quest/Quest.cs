using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Titan.QuestSystem
{
    /// <summary>
    /// Run-time에서 진행되는 Quest 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
    public class Quest : ScriptableObject
    {
        public int QuestIndex;
        public string QuestID;
        public string QuestName;
        public string QuestDescription;
        public QuestType Type;
        /// <summary>
        /// Run-time에서 불러온다.
        /// </summary>
        [System.NonSerialized] public QuestStatus Status;
        public SuperRequirement QuestGoal;
        public SuperRequirement QuestAcceptCondition;
        public int QuestRewardGold;
        public int QuestRewardExp;
        public List<string> QuestRewardItems = new();
        public List<string> QuestCompleteTriggerEvents = new();

        [ContextMenu("Test")]
        public void TestMethod()
        {
            Debug.Log("Json Utility");
            Debug.Log(JsonUtility.ToJson(this, true));
            Debug.Log($"Newton json");
            Debug.Log(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
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

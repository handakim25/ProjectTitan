using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{   
    /// <summary>
    /// Run-Time 다이얼로그의 한 부분
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        public string NodeID;
        public string NextNode;
        public string SpeakerName;
        public string SentenceText;
        /// <summary>
        /// 해당 노드가 발생시킬 트리거 ID
        /// </summary>
        public string TriggerEventID;
        public bool TriggerSetValue;
        /// <summary>
        /// 해당 노드가 발생시킬 퀘스트 ID
        /// </summary>
        public string TriggerQuest;
        public string TriggerQuestState;

        public List<Choice> Choices = new();
    }
}

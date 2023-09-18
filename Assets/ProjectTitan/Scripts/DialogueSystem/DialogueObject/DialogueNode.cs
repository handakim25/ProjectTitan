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
        public string DialogueText;
        public string TriggerEventID;
        public bool TriggerSetValue;
        public string TriggerQuest;
        public string TriggerQuestState;

        public List<Choice> Choices = new();
    }
}

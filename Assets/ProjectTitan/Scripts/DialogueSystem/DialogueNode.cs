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
        public string SpeakerName;
        public string DialogueText;
        public string NextNode;
        public string TriggerEventID;
        public bool TriggerValue;

        public List<Choice> choices = new();
    }
}

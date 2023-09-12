using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    [System.Serializable]
    public class Choice
    {
        public string ChoiceText;
        public string NextNode;
        public Condition Condition;
    }

    /// <summary>
    /// Event Condition만 일단 구현
    /// </summary>
    public class Condition
    {
        public string TriggerName;
        public bool ExpectedBool;
        public bool CheckCondition()
        {
            // get tirgger manager
            return true;
        }
    }
}

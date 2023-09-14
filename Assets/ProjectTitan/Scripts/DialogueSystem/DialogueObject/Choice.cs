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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    public abstract class Requirement
    {
        /// <summary>
        /// Expect Bool과 같으면 True 다르면 Flase이다.
        /// </summary>
        public bool ExpectedBool;
        public abstract bool IsMet(ConditionEvaluator conditionEvaluator);
    }

    public class TriggerRequirement : Requirement
    {
        public string TriggerEventID;

        public override bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            return conditionEvaluator.CheckTriggerCondition(TriggerEventID) == ExpectedBool;
        }
    }

    public class ItemRequirement : Requirement
    {
        public string ItemID;
        public int ItemCount;

        public override bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            return conditionEvaluator.CheckItemCondition(ItemID, ItemCount) == ExpectedBool;
        }
    }
}

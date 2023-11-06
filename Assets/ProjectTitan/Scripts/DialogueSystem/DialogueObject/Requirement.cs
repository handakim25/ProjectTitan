using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Titan
{
    [System.Serializable]
    public class Requirement
    {
        // 다형성을 지원하기는 어려울 것 같다.
        // 지금은 단일 클래스로 처리해 둔다.
        public enum RequirementType
        {
            Trigger,
            Item,
            MonsterKill,
            Quest,
        }
        public RequirementType Type;
        public bool ExpectedBool = true;
        public string TargetID;
        public int TargetCount;
        public QuestSystem.QuestStatus QuestStatus = QuestSystem.QuestStatus.NotReceived; // For Quest Only

        public bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            return Type switch
            {
                RequirementType.Quest => ExpectedBool == CheckQuestCondition(conditionEvaluator),
                RequirementType.Item => ExpectedBool == CheckItemCondition(conditionEvaluator),
                _ => false,
            };
        }

        private bool CheckQuestCondition(ConditionEvaluator conditionEvaluator)
        {
            return conditionEvaluator.CheckQuestConditon(TargetID, QuestStatus);
        }

        private bool CheckItemCondition(ConditionEvaluator conditionEvaluator)
        {
            return conditionEvaluator.CheckItemCondition(TargetID, TargetCount);
        }
    }
}

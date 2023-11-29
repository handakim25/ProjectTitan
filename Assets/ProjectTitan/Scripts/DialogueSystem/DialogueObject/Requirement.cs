using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Titan
{
    // @After-work
    // 1. 보편적으로 사용되는 파일이므로 namepsace를 제대로 지정하고 옳바른 디렉토리로 변경할 것
    // 2. public으로 값이 변경되는 건 좋은 설계가 아니다. 접근을 제어할 것.
    [System.Serializable]
    public class Requirement
    {
        // @Note
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
        /// <summary>
        /// TargetID는 각 Type에 따라 다른 의미를 가진다.Trigger : TriggerID, Item : ItemID, MonsterKill : MonsterID, Quest : QuestID
        /// </summary>
        public string TargetID;
        /// <summary>
        /// TargetCount는 각 Type에 따라 다른 의미를 가진다. Item : ItemCount, MonsterKill : KillCount
        /// </summary>
        public int TargetCount;
        /// <summary>
        /// Quest 조건
        /// </summary
        public QuestSystem.QuestStatus QuestStatus = QuestSystem.QuestStatus.NotReceived; // For Quest Only

        /// <summary>
        /// Condition을 만족하는지 확인한다. 만약 Expected Bool을 만족시켰을 경우 true를 반환한다.
        /// </summary>
        /// <param name="conditionEvaluator"></param>
        /// <returns></returns>
        public bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            return Type switch
            {
                RequirementType.Trigger => ExpectedBool == CheckTriggerCondition(conditionEvaluator),
                RequirementType.Quest => ExpectedBool == CheckQuestCondition(conditionEvaluator),
                RequirementType.Item => ExpectedBool == CheckItemCondition(conditionEvaluator),
                _ => false,
            };
        }

        private bool CheckTriggerCondition(ConditionEvaluator conditionEvaluator)
        {
            return conditionEvaluator.CheckTriggerCondition(TargetID);
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

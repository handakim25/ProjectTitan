using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    [System.Serializable]
    public abstract class Requirement
    {
        /// <summary>
        /// Expect Bool과 같으면 True 다르면 Flase이다.
        /// </summary>
        public bool ExpectedBool;
        public abstract bool IsMet(ConditionEvaluator conditionEvaluator);
        public void RegisterCondtionEvaluator(ConditionEvaluator conditionEvaluator)
        {

        }
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

    public class MonsterKillRequirement : Requirement
    {
        public string MonsterID;
        public int KillCount;

        public override bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            // return conditionEvaluator.CheckMonsterKillCondition(MonsterID, KillCount) == ExpectedBool;
            return true;
        }
    }

    [System.Serializable]
    public class SuperRequirement
    {
        [System.Serializable]
        public enum RequirementType
        {
            Trigger,
            Item,
            MonsterKill,
        }

        public RequirementType Type;
        private string Requirement;
        public bool ExpectedBool;
        public string RequireID;
        public int RequireCount;

        public void OnBeforeSerialize()
        {
            Requirement = Type.ToString();
        }

        public void OnAfterDeserialize()
        {
            throw new System.NotImplementedException();
        }
    }
}

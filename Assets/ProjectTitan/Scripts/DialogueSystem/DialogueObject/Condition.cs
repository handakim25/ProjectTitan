using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    [System.Serializable]
    public class Condition
    {
        public enum ConditionType
        {
            True,
            If,
            Any,
            All,
        }

        public ConditionType Type;
        public bool ExpectedBool;
        public List<Requirement> Requirements = new();
        
        // Check Requirements with condition type
        public bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            switch (Type)
            {
                case ConditionType.True:
                    return true;
                case ConditionType.If:
                    return Requirements[0].IsMet(conditionEvaluator);
                case ConditionType.Any:
                    foreach (var requirement in Requirements)
                    {
                        if (requirement.IsMet(conditionEvaluator))
                        {
                            return true;
                        }
                    }
                    return false;
                case ConditionType.All:
                    foreach (var requirement in Requirements)
                    {
                        if (!requirement.IsMet(conditionEvaluator))
                        {
                            return false;
                        }
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
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
        [SerializeField] public List<Requirement> Requirements = new();
        
        // Check Requirements with condition type
        public bool IsMet(ConditionEvaluator conditionEvaluator)
        {
            switch (Type)
            {
                case ConditionType.True:
                    return true;
                case ConditionType.If:
                    return Requirements.FirstOrDefault()?.IsMet(conditionEvaluator) ?? false;
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

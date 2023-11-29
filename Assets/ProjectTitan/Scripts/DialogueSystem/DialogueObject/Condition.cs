using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    // @After-work
    // Requrement와 마찮가지로 추후에 적절한 위치로 옮기고 접근 제어를 수정할 것
    /// <summary>
    /// 조건을 나타내는 클래스. 여러개의 필요 조건(Requrement)을 가질 수 있다.
    /// </summary>
    [System.Serializable]
    public class Condition
    {
        /// <summary>
        /// Requirement를 평가하는 방법
        /// </summary>
        public enum ConditionType
        {
            True, // 항상 True
            If, // 첫번째 Requirement만 확인
            Any, // Requirement 중 하나라도 만족하면 True
            All, // Requirement 모두 만족하면 True
        }

        public ConditionType Type;
        public bool ExpectedBool;
        public List<Requirement> Requirements = new(); 
        
        // Condition Type에 따라 평가
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

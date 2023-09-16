using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class ConditionEvaluator
    {
        public ConditionEvaluator()
        {

        }

        public bool CheckTriggerCondition(string TriggerID)
        {
            return true;
        }

        /// <summary>
        /// 아이템을 가지고 있는지 확인한다.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ItemCount"></param>
        /// <returns>ItemCount보다 많거나 같으면 True, 아니면 False</returns>
        public bool CheckItemCondition(string ItemID, int ItemCount)
        {
            return true;
        }
    }
}

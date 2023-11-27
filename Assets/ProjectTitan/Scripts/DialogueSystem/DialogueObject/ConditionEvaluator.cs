using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    /// <summary>
    /// 조건을 평가하는 클래스. 각 객체에서 직접하지 않고 이 클래스를 통해 평가한다.
    /// </summary>
    public class ConditionEvaluator
    {
        public QuestSystem.QuestManager QuestManager;
        public InventorySystem.Items.InventoryManager InventoryManager;
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
            if(InventoryManager == null)
            {
                return false;
            }
            var count = InventoryManager.GetItemCount(ItemID);
            return ItemCount <= count;
        }

        public bool CheckQuestConditon(string QuestID, QuestSystem.QuestStatus QuestStatus)
        {
            if(QuestManager == null)
            {
                return false;
            }
            return QuestStatus == QuestManager.GetQuestStatus(QuestID);
        }
    }
}

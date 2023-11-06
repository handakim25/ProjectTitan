using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
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

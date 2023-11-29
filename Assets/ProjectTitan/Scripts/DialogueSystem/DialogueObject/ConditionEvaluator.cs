using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    /// <summary>
    /// 조건을 평가하는 클래스. 각 객체에서 직접하지 않고 이 클래스를 통해 평가한다.
    /// 만약 적절한 manager가 없을 경우에는 flase를 반환한다.
    /// </summary>
    public class ConditionEvaluator
    {
        public GameEventSystem.GameEventManager GameEventManager;
        public QuestSystem.QuestManager QuestManager;
        public InventorySystem.Items.InventoryManager InventoryManager;
        public ConditionEvaluator()
        {

        }

        /// <summary>
        /// Game Event를 확인한다.
        /// </summary>
        /// <param name="TriggerID"></param>
        /// <returns></returns>
        public bool CheckTriggerCondition(string TriggerID)
        {
            if(GameEventManager == null)
            {
                return false;
            }
            GameEventManager.GetEventStatus(TriggerID, out bool status);
            return status;
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

        /// <summary>
        /// Quest 상태를 확인한다.
        /// </summary>
        /// <param name="QuestID">Quest ID</param>
        /// <param name="QuestStatus">Quest Status</param>
        /// <returns></returns>
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

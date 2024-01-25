using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.InventorySystem.Items
{
    sealed public class InventoryManager : MonoSingleton<InventoryManager>
    {
        [SerializeField] private InventoryObject _playerInventory;

        public InventoryObject InventoryObject => _playerInventory;

        private void Start()
        {
            EventBus.RegisterCallback<ItemCollectedEvent>(OnItemCollected);
        }

        private void OnItemCollected(ItemCollectedEvent e)
        {
            Debug.Log($"Item Collecteded");

            var item = new Item() {id = e.ItemID};
            _playerInventory.AddItem(item, e.Count);
        }

        /// <summary>
        /// 아이템을 가지고 있는지 확인한다.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>소지 아이템 개수를 반환. 잘못된 ID일 경우 -1 반환</returns>
        public int GetItemCount(string itemID)
        {
            if(!int.TryParse(itemID, out var intId))
            {
                return -1;
            }
            var item = new Item() {id = intId};
            var slot = _playerInventory.FindItemInInventory(item);
            if (slot == null)
            {
                return 0;
            }

            return slot.amount;
        }
    }
}

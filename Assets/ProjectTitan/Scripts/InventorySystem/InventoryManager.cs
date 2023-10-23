using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.InventorySystem.Items
{
    public class InventoryManager : MonoSingleton<InventoryManager>
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
    }
}

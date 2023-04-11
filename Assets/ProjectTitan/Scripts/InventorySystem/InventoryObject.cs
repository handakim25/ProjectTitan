using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Varialbes
        [SerializeField] protected ItemDatabase itemDatabase;
        [SerializeField] protected int _maxCapacity;

        // @ToDo : Create custom eidtor for Inventory.
        [SerializeField] Inventory inventory;
        // [SerializeField] 

        // @refactor
        // Pass changed arguemnts.
        // So that, it does not need to update all slots.
        [System.NonSerialized]
        public System.Action OnInventoryChanged;

        #endregion Varialbes

        #region Properties
        
        public List<InventorySlot> Slots => inventory.Slots;
        public int Capacity => _maxCapacity;
        public int ItemCount => Slots.Count;
        public bool IsFull => ItemCount >= Capacity;

        #endregion Properties

        #region Methods
        
        public bool AddItem(Item item, int amount)
        {
            InventorySlot slot = FindItemInInventory(item);
            if(!itemDatabase.itemObjects[item.id].stackable || slot == null)
            {
                if(IsFull)
                {
                    return false;
                }

                inventory.AddItem();
            }
            else
            {
                slot.AddAmount(amount);
            }

            return true;
        }

        public bool RemoveItem(Item item, int amount)
        {
            throw new System.NotImplementedException();
        }

        private InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(itemSlot => itemSlot.item.id == item.id);
        }

        #endregion Methods
#if UNITY_EDITOR
        #region TestMethods

        public void AddRandomItem()
        {
            Debug.Log($"Add random Item");
            if(itemDatabase.itemObjects.Length == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, itemDatabase.itemObjects.Length);
            ItemObject newItemObject = itemDatabase.itemObjects[Random.Range(0, itemDatabase.itemObjects.Length)];
            Item newItem = new Item(newItemObject);
            AddItem(newItem, 1);
        }

        #endregion TestMethods
#endif
    }
}

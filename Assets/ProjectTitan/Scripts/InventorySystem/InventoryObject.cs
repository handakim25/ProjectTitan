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

        public class SlotCountChangedEventArgs : System.EventArgs
        {
            public List<InventorySlot> UpdatedSlots;

            public SlotCountChangedEventArgs(List<InventorySlot> slots)
            {
                UpdatedSlots = slots;
            }
        }

        public delegate void SlotCountChangedEventHandler(Object sender, SlotCountChangedEventArgs args);
        [System.NonSerialized]
        public SlotCountChangedEventHandler OnSlotCountChanged;

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
                // a. stackable true, slot == null : Item does not exist.
                // b. stackable false, slot == null : Item does not exist.
                // c. stackable true, slot != null : go to else, Add item to an existing slot.
                // d. stackable false, slot != null : The item exists but it is not stackable, create a new slot.
                if(IsFull)
                {
                    return false;
                }

                var addedSlot = inventory.AddItem(item, amount);
                var args = new SlotCountChangedEventArgs(new List<InventorySlot> { addedSlot });
                OnSlotCountChanged?.Invoke(this, args);
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

        // As the appropriate access modifer is unclear at this time,
        // it will be kept private for now.
        private InventorySlot FindItemInInventory(Item item)
        {
            return inventory.FindItemInInventory(item);
        }

        #endregion Methods
#if UNITY_EDITOR
        #region TestMethods

        public void AddRandomItem()
        {
            if(itemDatabase.itemObjects.Length == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, itemDatabase.itemObjects.Length);
            ItemObject newItemObject = itemDatabase.itemObjects[randomIndex];
            Item newItem = new Item(newItemObject);
            Debug.Log($"Create item / id : {newItem.id}");
            AddItem(newItem, 1);
        }

        #endregion TestMethods
#endif
    }
}

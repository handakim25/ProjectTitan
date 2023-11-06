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
        [SerializeField] protected int _maxCapacity;

        // @ToDo : Create custom eidtor for Inventory.
        [SerializeField] Inventory inventory;

        public class SlotCountChangedEventArgs : System.EventArgs
        {
            public List<InventorySlot> UpdatedSlots;
            public List<InventorySlot> RemovedSlots;
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
            if (!DataManager.ItemDatabase.GetItemObject(item.id).stackable || slot == null)
            {
                // a. stackable true, slot == null : Item does not exist.
                // b. stackable false, slot == null : Item does not exist.
                // c. stackable true, slot != null : go to else, Add item to an existing slot.
                // d. stackable false, slot != null : The item exists but it is not stackable, create a new slot.
                if (IsFull)
                {
                    return false;
                }

                var addedSlot = inventory.AddItem(item, amount);
                var args = new SlotCountChangedEventArgs()
                {
                    UpdatedSlots = new List<InventorySlot>() { addedSlot }
                };
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

        public InventorySlot FindItemInInventory(Item item)
        {
            return inventory.FindItemInInventory(item);
        }

        public bool RemoveItem(InventorySlot slotToUse, int amount)
        {
            if (!slotToUse.IsValid || slotToUse.amount < amount)
            {
                return false;
            }

            ItemObject itemObject = DataManager.ItemDatabase.GetItemObject(slotToUse.item.id);
            slotToUse.UpdateSlot(slotToUse.item, slotToUse.amount - amount);

            if (slotToUse.amount <= 0)
            {
                Debug.Log($"Invoke SlotCountChanged");
                inventory.RemoveSlot(slotToUse);
                var eventArgs = new SlotCountChangedEventArgs()
                {
                    RemovedSlots = new List<InventorySlot> { slotToUse }
                };
                OnSlotCountChanged?.Invoke(this, eventArgs);
            }

            return true;
        }

        #endregion Methods
#if UNITY_EDITOR
        #region TestMethods

        public void AddRandomItem()
        {
            if (DataManager.ItemDatabase.Length == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, DataManager.ItemDatabase.Length);
            ItemObject newItemObject = DataManager.ItemDatabase.GetItemObject(randomIndex);
            Item newItem = new Item(newItemObject);
            Debug.Log($"Create item / id : {newItem.id}");
            AddItem(newItem, 1);
        }

        #endregion TestMethods
#endif
    }
}

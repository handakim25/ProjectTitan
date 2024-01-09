using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    /// <summary>
    /// Inventory 데이터를 가지고 있는 ScriptableObject
    /// </summary>
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

        /// <summary>
        /// Item을 추가한다. Stackable한 Item이라면 기존에 있는 Item의 개수를 늘린다.
        /// 만약 Item이 없다면 새로운 Slot을 생성한다.
        /// </summary>
        /// <param name="item">추가할 Item</param>
        /// <param name="amount">추가할 수량</param>
        /// <returns>인벤토리가 가득 찼을 경우 실패하며 False를 반환</returns>
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

        /// <summary>
        /// Item ID와 일치하는 Item을 찾아서 반환한다. 가장 처음에 일치하는 Item을 반환한다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>찾지 못했을 경우 null을 반환</returns>
        public InventorySlot FindItemInInventory(Item item)
        {
            return inventory.FindItemInInventory(item);
        }

        /// <summary>
        /// Slot의 아이템을 개수만큼 제거한다. 만약 더 많은 개수를 제거하려고 한다면 false를 반환한다.
        /// 개수가 0이 되면 Slot을 제거한다.
        /// </summary>
        /// <param name="slotToUse"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Item ID와 일치하는 Item을 찾아서 개수만큼 제거한다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Item이 없거나, 더 많은 개수를 제거할려면 False를 반환</returns>
        public bool RemoveItem(Item item, int amount)
        {
            InventorySlot slot = FindItemInInventory(item);
            if (slot == null)
            {
                return false;
            }

            return RemoveItem(slot, amount);
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
            Item newItem = new(newItemObject);
            Debug.Log($"Create item / id : {newItem.id} / name : {newItemObject.ItemName}");
            AddItem(newItem, 1);
        }

        #endregion TestMethods
#endif
    }
}

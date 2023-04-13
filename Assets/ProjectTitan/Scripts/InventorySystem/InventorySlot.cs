using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        #region Variables

        [NonSerialized] public GameObject SlotUI;

        [NonSerialized] public Action<InventorySlot> OnPreUpdate;
        [NonSerialized] public Action<InventorySlot> OnPostUpdate;

        public Item item;
        public int amount;    

        #endregion Variables

        #region Methods

        public InventorySlot() => UpdateSlot(new Item(), 0);
        public InventorySlot(Item item, int amount) => UpdateSlot(item, amount);

        /// <summary>
        /// Update Slot
        /// If amount is less then 0, it will be empty slot
        /// OnPreUpdate and OnPostUpdate will be called
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void UpdateSlot(Item item, int amount)
        {
            if(amount < 0)
            {
                item = Item.NullItem;
            }

            OnPreUpdate?.Invoke(this);
            this.item = item;
            this.amount = amount;
            OnPostUpdate?.Invoke(this);
        }

        public  void AddAmount(int value) => UpdateSlot(item, amount += value);

        #endregion Methods
    }
}

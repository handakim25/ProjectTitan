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

        [NonSerialized]
        public Action<InventorySlot> OnPreUpdate;
        [NonSerialized]
        public Action<InventorySlot> OnPostUpdate;

        public Item item;
        public int amount;    

        #endregion Variables

        #region Methods

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

        #endregion Methods
    }
}

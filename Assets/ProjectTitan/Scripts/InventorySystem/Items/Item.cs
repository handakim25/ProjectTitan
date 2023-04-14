using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    /// <summary>
    /// Instance of ItemObject.
    /// Also, represent data of ItemObject.
    /// </summary>
    [System.Serializable]
    public class Item
    {
        /// <summary>
        /// Unique number of item.
        /// Key of ItemDatabase.
        /// If id == -1, Empty Item.
        /// </summary>
        public int id;

        public Item()
        {
            id = -1;
        }

        public Item(ItemObject itemObject)
        {
            id = itemObject.data.id;
        }

         public static Item NullItem {
            get => new Item();
        }

        public Item Clone()
        {
            return new Item() {id = this.id};
        }
    }
}

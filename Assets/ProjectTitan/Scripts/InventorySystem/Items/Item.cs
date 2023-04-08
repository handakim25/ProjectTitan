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
        public int id = -1;

        public Item()
        {
            id = -1;
        }

        public Item(ItemObject itemObject)
        {
            id = itemObject.data.id;
        }
    }
}

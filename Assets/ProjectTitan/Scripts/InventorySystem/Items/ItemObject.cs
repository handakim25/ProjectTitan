using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/New Item")]
    public class ItemObject : ScriptableObject
    {
        #region Variables
        public string ItemName;
        public ItemType type;
        public bool stackable;

        [Tooltip("Icon that will be displayed on Inventory UI")]
        public Sprite icon;

        [TextArea(5,20)]
        [Tooltip("Description that will be on Inventory Detail UI")]
        public string description;

        public Item data = new();

        #endregion Variables
    }
}
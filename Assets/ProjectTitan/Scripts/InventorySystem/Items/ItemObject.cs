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
        public ItemRarity rarity = ItemRarity.Common;
        public bool stackable;

        [Tooltip("인벤토리 UI에서 보여질 아이콘 이미지")]
        public Sprite icon;

        [TextArea(5,20)]
        [Tooltip("아이템 창에서 보여질 설명")]
        public string description;

        public Item data = new();

        #endregion Variables
    }
}
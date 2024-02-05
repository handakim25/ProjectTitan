using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    public class FilterSelector : MonoBehaviour
    {
        [SerializeField] private ItemType _itemType;
        public ItemType ItemType => _itemType;
    }
}

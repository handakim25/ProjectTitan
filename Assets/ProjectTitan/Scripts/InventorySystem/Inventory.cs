using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    /// <summary>
    /// Wrapper of Inventory Container
    /// Logic is on InventoryObject
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        #region Variables

        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        #endregion Variables

        #region Properties
        
        // @Refactor
        // Modifying the implementation to allow for the use of other data structure.
        public List<InventorySlot> Slots => slots;
        #endregion Properties

        #region Methods
        
        public InventorySlot AddItem(Item item, int amount)
        {
            var newSlot = new InventorySlot(item, amount);
            slots.Add(newSlot);
            return newSlot;
        }

        public InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(itemSlot => itemSlot.item.id == item.id);
        }
        
        #endregion Methods
    }
}

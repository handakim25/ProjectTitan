using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem
{
    /// <summary>
    /// Wrapper of Inventory slot
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        #region Variables
        public readonly int MaxSlots = 500;

        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        #endregion Variables

        #region Properties
        
        // @Refactor
        // Modifying the implementation to allow for the use of other data structure.
        public List<InventorySlot> Slots => slots;
        #endregion Properties
    }
}

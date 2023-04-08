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

        private List<InventorySlot> slots = new List<InventorySlot>();

        #endregion Variables
    }
}

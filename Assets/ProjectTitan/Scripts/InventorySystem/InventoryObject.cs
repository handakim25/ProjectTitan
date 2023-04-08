using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Varialbes
        
        // @refactor
        // Pass changed arguemnts.
        // So that, it does not need to update all slots.
        public event System.Action OnInventoryChanged;

        #endregion Varialbes

        #region Methods
        
        public bool AddItem(Item item, int amount)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(Item item, int amount)
        {
            throw new System.NotImplementedException();
        }

        #endregion Methods
    }
}

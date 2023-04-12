using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI.InventorySystem
{
    public class InventoryUIController : UIBase
    {
        [SerializeField] protected InventoryUI inventoryUI;

#if UNITY_EDITOR
        public Titan.InventorySystem.InventoryObject inventoryObject;
        // Test methods for testing
        public void AddRandomItem()
        {
            inventoryObject.AddRandomItem();
        }
#endif
    }
}

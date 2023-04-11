using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI.InventorySystem
{
    public class InventoryUIController : UIBase
    {
        [SerializeField] protected InventoryUI inventoryUI;
        
        public void CloseUI()
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Titan.InventorySystem;
using Titan.InventorySystem.Items;

namespace Titan.UI.InventorySystem
{
    public class InventoryUIController : UIBase
    {
        [SerializeField] protected InventoryUI inventoryUI;
        [SerializeField] protected GameObject DetailSlotUI;
        private InventorySlot _detailSlot = new InventorySlot();

        #region UnityMethods

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(inventoryUI, "Inventory ui is not set");
            UnityEngine.Assertions.Assert.IsNotNull(DetailSlotUI, "Detail Slot Ui is not set");

            _detailSlot.SlotUI = DetailSlotUI;
            _detailSlot.OnPostUpdate += OnDetailSlotPostUpdate;
            
            Debug.Log($"Awake set emtpy detail");
            _detailSlot.UpdateSlot(new Item(), 0);

            inventoryUI.OnSlotSelected += (slot) => {
                if(slot!=null)
                {
                    _detailSlot.UpdateSlot(slot.item, 0);
                }
            };            
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            InventorySlot firstSlot = inventoryUI.GetFirstSlot();
            if(firstSlot==null)
            {
                Debug.Log($"OnEnable set empty detail");
                _detailSlot.UpdateSlot(new Item(), 0);
            }
            else
            {
                Debug.Log($"OnEnable Get first slot");
                _detailSlot.UpdateSlot(firstSlot.item, firstSlot.amount);
            }
        }

        #endregion UnityMethods

        #region Callback
        
        private void OnDetailSlotPostUpdate(InventorySlot slot)
        {
            Debug.Log($"Item id : {slot.item.id}");
            if(!slot.IsValid)
            {
                Debug.Log($"Deslect is not implemented");
                return;
            }

            var itemObject = UIManager.Instance.GetItemObject(slot.item.id);
            
            var ItemText = _detailSlot.SlotUI.transform.Find("UpperBar/ItemNameText").gameObject;
            if(ItemText != null)
            {
                ItemText.GetComponent<TMP_Text>().text = itemObject.name;
            }
        }


        #endregion Callback
#region TestMethods in editor
    #if UNITY_EDITOR
            [Tooltip("Test object only valid in editor mode")]
            public Titan.InventorySystem.InventoryObject inventoryObject;
            // Test methods for testing
            public void AddRandomItem()
            {
                inventoryObject.AddRandomItem();
            }
    #endif
#endregion TestMethods    
    }
}

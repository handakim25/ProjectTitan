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
        [SerializeField] InventoryObject _inventoryObject;
        [SerializeField] protected InventoryUI _inventoryUI;
        [SerializeField] protected GameObject DetailSlotUI;
        private InventorySlot _detailSlot = new InventorySlot();

        #region UnityMethods

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryObject, "Inventory is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryUI, "Inventory ui is not set");
            UnityEngine.Assertions.Assert.IsNotNull(DetailSlotUI, "Detail Slot Ui is not set");

            // setup detail slot
            _detailSlot.SlotUI = DetailSlotUI;
            _detailSlot.OnPostUpdate += OnDetailSlotPostUpdate;
            
            _detailSlot.UpdateSlot(new Item(), 0);

            _inventoryUI.OnSlotSelected += (InventorySlot slot) => {
                _detailSlot.UpdateSlot(slot.item.Clone(), 0);
            };
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            _inventoryUI.CreateSlots(_inventoryObject.Slots); // slots will be destroyed OnDisable
            _inventoryObject.OnSlotCountChanged += OnSlotCountChangedHandler;
            
            var firstSlot = _inventoryUI.GetFirstSlotGo();
            _inventoryUI.SelectSlot(firstSlot);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            _inventoryObject.OnSlotCountChanged -= OnSlotCountChangedHandler;
        }

        #endregion UnityMethods

        #region Callback

        protected void OnSlotCountChangedHandler(object e, InventoryObject.SlotCountChangedEventArgs handler)
        {
            _inventoryUI.CreateSlots(handler.UpdatedSlots);
        }  

        private void OnDetailSlotPostUpdate(InventorySlot slot)
        {
            if(!slot.IsValid)
            {
                Debug.Log($"Deslect is not implemented");
                var text = _detailSlot.SlotUI.transform.Find("UpperBar/ItemNameText").gameObject;
                if(text != null)
                {
                    text.GetComponent<TMP_Text>().text = "name";
                }
                return;
            }

            var itemObject = UIManager.Instance.GetItemObject(slot.item.id);
            
            var ItemText = _detailSlot.SlotUI.transform.Find("UpperBar/ItemNameText").gameObject;
            if(ItemText != null)
            {
                ItemText.GetComponent<TMP_Text>().text = itemObject.name;
            }
        }

        public void OnDetailButtonClicked()
        {
            Debug.Log($"Detail Button");
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

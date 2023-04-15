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
        [Header("Inventory")]
        [SerializeField] InventoryObject _inventoryObject;

        [Header("UI")]
        [SerializeField] protected InventoryUI _inventoryUI;
        [SerializeField] protected GameObject DetailSlotUI;
        private InventorySlot _detailSlot = new InventorySlot();
        [SerializeField] protected TMP_Text _capacityText;

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
                if(slot == null)
                {
                    _detailSlot.UpdateSlot(new Item(), 0);
                }
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
            if(firstSlot!=null)
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
            if(handler.UpdatedSlots != null)
                _inventoryUI.CreateSlots(handler.UpdatedSlots);
            if(handler.RemovedSlots != null)
            {
                if(handler.RemovedSlots.Contains(_inventoryUI.SelectedSlot))
                {
                    // Select other slot
                    // move right
                    
                }
                _inventoryUI.RemoveSlots(handler.RemovedSlots);
                // _inventoryUI.SelectSlot(null);
            }
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

            var itemObject = ItemDatabase.GetItemObject(slot.item.id);
            
            if(_detailSlot.SlotUI.TryGetComponent<SlotUI>(out var slotUI))
            {
                slotUI.ItemNameText = itemObject.name;
                slotUI.IconImage = itemObject.icon;
                slotUI.ItemDescText = itemObject.description;
                slotUI.ItemTypeText = itemObject.type.ToString();
            }
        }

        public void OnInteractionClick()
        {
            InventorySlot slectedSlot = _inventoryUI.SelectedSlot;
            Debug.Log($"Selected Item: {_inventoryUI.SelectedSlot.SlotUI.name}");

            _inventoryObject.RemoveItem(slectedSlot, 1);
        }

        #endregion Callback
    
#region TestMethods in editor
    #if UNITY_EDITOR
            // Test methods for testing
            public void AddRandomItem()
            {
                _inventoryObject.AddRandomItem();
            }
    #endif
#endregion TestMethods    
    }
}

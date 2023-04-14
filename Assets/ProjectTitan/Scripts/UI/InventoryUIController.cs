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
                _inventoryUI.RemoveSlots(handler.RemovedSlots);
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

        public void OnInteractionClick()
        {
            InventorySlot slectedSlot = _inventoryUI.SelectedSlot;
            Debug.Log($"Selected Item: {_inventoryUI.SelectedSlot.SlotUI.name}");

            _inventoryObject.RemoveItem(slectedSlot, 1);
            if(_inventoryUI.SelectedSlot == null)
            {
                Debug.Log($"Try select first");
                var firstSlot = _inventoryUI.GetFirstSlotGo();
                if(firstSlot)
                {
                    Debug.Log($"Select first slot");
                    _inventoryUI.SelectSlot(firstSlot);
                    Debug.Log($"Slected slot : {_inventoryUI.SelectedSlot}");
                }
                else
                {
                    _detailSlot.UpdateSlot(new Item(), 0);
                }
            }
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

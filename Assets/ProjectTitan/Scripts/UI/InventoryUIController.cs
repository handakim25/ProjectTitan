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
            
            _detailSlot.UpdateSlot(new Item(), 0);
        }

        // Overview
        // 1. First Open
        //  a. InventoryUIController, InventryUI Awake, OnEnable
        //  b. InventoryUIController Start, Listen InventoryUI.OnSlotCreated
        //  c. InventoryUIController Start, Select First Slot
        // 2. Second Open
        //  a. InventoryUIController, InventoryUI OnEnable
        //  b. InventoryUI OnEnable, CreateSLot -> OnSlotCreated -> InventoryUIController
        //  c. InvnentoryUIController<- OnSlotCreated : Selct First Slot
        private void Start()
        {
            inventoryUI.OnSlotSelected += (slot) => {
                if(slot!=null)
                {
                    _detailSlot.UpdateSlot(slot.item, 0);
                }
            };   

            inventoryUI.OnSlotCreated += (InventoryUI inventoryUI) => {
                GameObject firstSlotGo = inventoryUI.GetFirstSlotGo();
                if(firstSlotGo == null)
                {
                    _detailSlot.UpdateSlot(new Item(), 0);
                    return;
                }

                inventoryUI.SetSlotSelected(firstSlotGo);
                var slot = inventoryUI.GetSlotByGo(firstSlotGo);
                var item = slot.item.Clone();
                _detailSlot.UpdateSlot(slot.item, 0);
            };        

            GameObject firstSlotGo = inventoryUI.GetFirstSlotGo();
            inventoryUI.SetSlotSelected(firstSlotGo);
            var slot = inventoryUI.GetSlotByGo(firstSlotGo);
            var item = slot.item.Clone();
            _detailSlot.UpdateSlot(slot.item, 0);    
        }

        #endregion UnityMethods

        #region Callback
        
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

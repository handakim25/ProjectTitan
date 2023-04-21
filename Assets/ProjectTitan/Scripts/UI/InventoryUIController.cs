using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Titan.InventorySystem;
using Titan.InventorySystem.Items;

namespace Titan.UI.InventorySystem
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("Inventory")]
        [SerializeField] InventoryObject _inventoryObject;

        [Header("UI")]
        [SerializeField] protected GameObject _cartegoryTab;
        [SerializeField] protected InventoryUI _inventoryUI;
        [SerializeField] protected GameObject _detailSlotUI;
        private InventorySlot _detailSlot = new InventorySlot();
        [SerializeField] protected FormatString _capacityText;

        #region UnityMethods

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryObject, "Inventory is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_cartegoryTab, "Cartegory is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_cartegoryTab.GetComponent<TabGroup>(), "Cartegory tab should have TabGroupd");
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryUI, "Inventory ui is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_detailSlotUI, "Detail Slot Ui is not set");

            // setup detail slot
            _detailSlot.SlotUI = _detailSlotUI;
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

        // 순서에 주의할 것
        // OnEnable의 경우, UI 특성상 열고 닫히는 구조니까 세심하게 관리해야 한다.
        // 인벤토리 슬롯을 만들고, Cartegory를 선택해서 보여질 것들을 정리하고, 선택한다.
        private void OnEnable()
        {
            _inventoryUI.CreateSlots(_inventoryObject.Slots); // slots will be destroyed OnDisable
            _inventoryObject.OnSlotCountChanged += OnSlotCountChangedHandler;

            if(_cartegoryTab.transform.childCount > 0)
            {
                var firstCartegoryGo = _cartegoryTab.transform.GetChild(0);
                firstCartegoryGo.GetComponent<TabButton>()?.Select();
            }
            
            var firstSlot = _inventoryUI.GetFirstSlotGo();
            if(firstSlot!=null)
                _inventoryUI.SelectSlot(firstSlot);

            _capacityText.Format(_inventoryObject.ItemCount, _inventoryObject.Capacity);
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
                // @To-Do : Selection code.
                if(handler.RemovedSlots.Contains(_inventoryUI.SelectedSlot))
                {
                    // Select other slot
                    // move right
                }
                _inventoryUI.RemoveSlots(handler.RemovedSlots);
                // _inventoryUI.SelectSlot(null);
            }

            InventoryObject inventory = e as InventoryObject;
            _capacityText.Format(inventory.ItemCount, inventory.Capacity);
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

        public void OnCartegoryButton(bool isLeft)
        {
            int tabCount = _cartegoryTab.transform.childCount;
            if(tabCount == 0)
            {
                return;
            }

            TabButton selectedButton = _cartegoryTab.GetComponent<TabGroup>().SelectedTab;
            int index = selectedButton.transform.GetSiblingIndex();
            int newIndex = index + (isLeft ? -1 : 1);

            if(newIndex < 0) newIndex = tabCount - 1;
            else if(newIndex >= tabCount) newIndex = 0;

            GameObject selectedTab = _cartegoryTab.transform.GetChild(newIndex).gameObject;
            selectedTab.GetComponent<TabButton>()?.Select();
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

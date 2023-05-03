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
        [SerializeField] protected Button _interactButton;

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

            // @To-Do
            // 시나리오를 나누어서 생각할 것
            _inventoryUI.OnSlotSelected += (InventorySlot slot) => {
                if(slot == null)
                {
                    _detailSlot.UpdateSlot(new Item(), 0);
                    _interactButton.gameObject.SetActive(false);
                    return;
                }
                _detailSlot.UpdateSlot(slot.item.Clone(), 0);

                _interactButton.gameObject.SetActive(true);
                ItemObject item = ItemDatabase.GetItemObject(slot.item.id);
                TextMeshProUGUI text = _interactButton.GetComponentInChildren<TextMeshProUGUI>();
                text.text = item.type switch
                    {
                        ItemType.Weapon => "착용",
                        ItemType.Food => "사용",
                        _ => "상세",
                    };
            };

            // tab button -> Select -> Filter Inventory UI 
            // ->  TabGroup.OntabSelected -> OnTabSelectedEvent -> Select Slot
            // If cartegory is changed, select first slot
            _cartegoryTab.GetComponent<TabGroup>().OnTabSelectedEvent += (tabButton) => {
                var firstSlot = _inventoryUI.GetFirstSlotGo();
                _inventoryUI.SelectSlot(firstSlot);
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
                int index = _inventoryUI.SelectedSlot.SlotUI.transform.GetSiblingIndex();
                _inventoryUI.RemoveSlots(handler.RemovedSlots);
                if(_inventoryUI.SelectedSlot == null)
                {
                    GameObject nextSelectedSlot = SelectNextSlot(index);
                    _inventoryUI.SelectSlot(nextSelectedSlot);
                }
            }

            InventoryObject inventory = e as InventoryObject;
            _capacityText.Format(inventory.ItemCount, inventory.Capacity);
        }  

        private GameObject SelectNextSlot(int startIndex)
        {
            for(int i = startIndex; i < _inventoryUI.SlotCount; i++)
            {
                GameObject nextSlot = _inventoryUI.GetSlotByIndex(i);
                if(_inventoryUI.IsValidSlot(nextSlot))
                {
                    return nextSlot;
                }
            }
            for(int i = startIndex -1 ; i >= 0; i--)
            {
                GameObject nextSlot = _inventoryUI.GetSlotByIndex(i);
                if(_inventoryUI.IsValidSlot(nextSlot))
                {
                    return nextSlot;
                }
            }
            return null;
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
                var icon = _detailSlot.SlotUI.transform.Find("DetailIcon/IconImage");
                if(icon != null)
                {
                    icon.GetComponent<Image>().sprite = null;
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

        #endregion Callback

        #region Button Callback

        public void OnInteractionClick()
        {
            InventorySlot slectedSlot = _inventoryUI.SelectedSlot;

            if(slectedSlot != null)
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

        #endregion Button Callback
    
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

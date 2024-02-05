using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Titan.InventorySystem;
using Titan.InventorySystem.Items;

namespace Titan.UI.InventorySystem
{
    /// <summary>
    /// Invenotry UI의 Controller
    /// </summary>
    public class InventoryUIController : MonoBehaviour
    {
        [Header("Inventory")]
        [Tooltip("Player Inventory")]
        [SerializeField] InventoryObject _inventoryObject;

        [Header("UI/Header")]
        
        [SerializeField] protected GameObject _categoryTab;
        [SerializeField] protected TextMeshProUGUI _categoryText;
        [Tooltip("용량을 표현하기 위한 Text")]
        [SerializeField] protected FormatString _capacityText;

        [Header("UI/Body")]
        [Tooltip("Inventory 전체를 보여주는 UI")]
        [SerializeField] protected InventoryUI _inventoryUI;
        [Tooltip("선택된 아이템의 상세 정보를 보여주는 UI")]
        [SerializeField] protected GameObject _detailSlotUI;
        /// <summary>
        /// 선택된 아이템의 상세 정보를 보여주는 슬롯
        /// InventoryUI와는 다른 슬롯이다.
        /// </summary>
        private InventorySlot _detailSlot = new InventorySlot();
        [SerializeField] protected GameObject _equipIndicatorUI;

        [Header("UI/Footer")]
        [SerializeField] protected Button _interactButton;

        #region UnityMethods

        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryObject, "Inventory is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_categoryTab, "Cartegory is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_categoryTab.GetComponent<TabGroup>(), "Cartegory tab should have TabGroupd");
            UnityEngine.Assertions.Assert.IsNotNull(_categoryText, "Category Text is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_inventoryUI, "Inventory ui is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_detailSlotUI, "Detail Slot Ui is not set");
            UnityEngine.Assertions.Assert.IsNotNull(_equipIndicatorUI, "Equip Indicator Ui is not set");

            // Setup Detail Slot
            _detailSlot.SlotUI = _detailSlotUI;
            _detailSlot.OnPostUpdate += OnDetailSlotPostUpdate;
            _detailSlot.UpdateSlot(new Item(), 0);

            _inventoryUI.OnSlotSelected += (InventorySlot slot) => {
                if(slot == null)
                {
                    _detailSlot.UpdateSlot(new Item(), 0);
                    _interactButton.gameObject.SetActive(false);
                    _equipIndicatorUI.SetActive(false);
                    return;
                }
                _detailSlot.UpdateSlot(slot.item.Clone(), 0);
                ItemObject item = DataManager.ItemDatabase.GetItemObject(slot.item.id);

                _interactButton.gameObject.SetActive(true);
                TextMeshProUGUI text = _interactButton.GetComponentInChildren<TextMeshProUGUI>();
                text.text = item.type switch
                    {
                        ItemType.Weapon => "착용",
                        ItemType.Food => "사용",
                        _ => "상세",
                    };

                // _equipIndicatorUI.SetActive(true);
            };

            // tab button -> Select ->Deselect Other Tab -> OnTabDeselectedEvent -> Update Select tab -> OnTabSelectedEvent
            // 만약, Category가 바뀌면, 첫번째 슬롯을 선택한다.
            _categoryTab.GetComponent<TabGroup>().OnTabSelectedEvent += (tabButton) => {
                if(tabButton.TryGetComponent<FilterSelector>(out var filterSelector))
                {
                    _categoryText.text = filterSelector.ItemType.ToText();
                    _inventoryUI.SetFilter(filterSelector.ItemType);
                }
                else
                {
                    _categoryText.text = "All";
                    _inventoryUI.RemoveFilter();
                }

                var firstSlot = _inventoryUI.GetFirstSlotGo();
                _inventoryUI.SelectSlot(firstSlot);
            };

            _categoryTab.GetComponent<TabGroup>().OnTabDeselectedEvent += (tabButton) => {
                if(tabButton.TryGetComponent<FilterSelector>(out var filterSelector))
                {
                    _inventoryUI.RemoveFilter(filterSelector.ItemType);
                }
                else
                {
                    _inventoryUI.RemoveFilter();
                }
            };
        }

        // 순서에 주의할 것
        // OnEnable의 경우, UI 특성상 열고 닫히는 구조니까 세심하게 관리해야 한다.
        // 인벤토리 슬롯을 만들고, Cartegory를 선택해서 보여질 것들을 정리하고, 선택한다.
        private void OnEnable()
        {
            _inventoryUI.CreateSlots(_inventoryObject.Slots); // slots will be destroyed OnDisable
            _inventoryObject.OnSlotCountChanged += OnSlotCountChangedHandler;

            if (_categoryTab.transform.childCount > 0)
            {
                var firstCartegoryGo = _categoryTab.transform.GetChild(0);
                if (firstCartegoryGo.TryGetComponent<TabButton>(out var tabButton))
                {
                    tabButton.Select();
                }
            }

            UpdateCapacityText();
        }

        private void OnDisable()
        {
            _inventoryObject.OnSlotCountChanged -= OnSlotCountChangedHandler;
        }

        #endregion UnityMethods

        private void UpdateCapacityText()
        {
            _capacityText.Format(_inventoryObject.ItemCount, _inventoryObject.Capacity);
        }

        #region Callback

        /// <summary>
        /// Inventory Object에 변경이 있을 때 호출되는 콜백
        /// </summary>
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

            UpdateCapacityText();
        }  

        /// <summary>
        /// StartIndex를 기반으로 다음 슬롯을 선택한다.
        /// Start 다음부터 선택을 하며 없을 경우, Start 이전부터 선택한다.
        /// </summary>
        /// <param name="startIndex">시작할 인덱스</param>
        /// <returns>선택될 Index, 만약에 선택할 slot이 없다면 null</returns>
        private GameObject SelectNextSlot(int startIndex)
        {
            // 주의
            // Slot이 Destroy 되는 것은 Update 이후에 발생하므로 현 Update Loop 내애서도 접근이 가능하다.
            // 따라서 반드시 유효성을 확인해야 한다.
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
            // Empty Selected Slot
            if(!slot.IsValid)
            {
                Debug.Log($"Empty Slot");
                _detailSlot.SlotUI.SetActive(false);
                return;
            }

            var itemObject = DataManager.ItemDatabase.GetItemObject(slot.item.id);
            if(itemObject == null)
            {
                Debug.LogError($"ItemObject is not found. id: {slot.item.id}");
                return;
            }

            _detailSlot.SlotUI.SetActive(true);
            if(_detailSlot.SlotUI.TryGetComponent<SlotUI>(out var slotUI))
            {
                slotUI.ItemNameText = itemObject.ItemName;
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

            // @To-Do
            // 아이템 사용, 착용, 읽기 등을 구현
            // Think. 각각의 구현은 누가 담당해야 하는가?
            if(slectedSlot != null)
                _inventoryObject.RemoveItem(slectedSlot, 1);
        }

        /// <summary>
        /// Category Change Button Handler
        /// </summary>
        /// <param name="isLeft"></param>
        public void OnCartegoryButton(bool isLeft)
        {
            int tabCount = _categoryTab.transform.childCount;
            if(tabCount == 0)
            {
                return;
            }

            TabButton selectedButton = _categoryTab.GetComponent<TabGroup>().SelectedTab;
            int index = selectedButton.transform.GetSiblingIndex();
            int newIndex = index + (isLeft ? -1 : 1);

            if(newIndex < 0) newIndex = tabCount - 1;
            else if(newIndex >= tabCount) newIndex = 0;

            GameObject selectedTab = _categoryTab.transform.GetChild(newIndex).gameObject;
            if(selectedTab.TryGetComponent<TabButton>(out var tabButton))
            {
                tabButton.Select();
            }
        }

        #endregion Button Callback 
    }
}

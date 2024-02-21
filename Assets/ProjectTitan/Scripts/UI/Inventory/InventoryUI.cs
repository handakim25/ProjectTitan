using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using Titan.InventorySystem;
using Titan.InventorySystem.Items;

namespace Titan.UI.InventorySystem
{
    // @Refactor
    // Tween Button으로 기능 분할

    /// <summary>
    /// Inventory UI 중에서 Inventory Vierw만을 담당하는 클래스.
    /// Detail Slot은 다른 클래스에서 담당한다.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] GameObject _slotPrefab;
        [SerializeField] ScrollRect _inventoryScroll;
        /// <summary>
        /// 표시할 아이템 타입을 지정. None이면 모든 타입을 표시한다.
        /// </summary>
        [Tooltip("표시할 아이템 타입을 지정. None이면 모든 타입을 표시한다.")]
        [SerializeField] List<ItemType> _allowedType = new List<ItemType>();

        /// <summary>
        /// Key : Slot GameObject, Value : InventorySlot
        /// </summary>
        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();
        /// <summary>
        /// 마지막으로 생성된 슬롯의 인덱스. Scene에서 구분하기 위해서 사용될 뿐이며 생성될 때마다 늘어나기만 한다.
        /// </summary>
        private int _lastSlotIndex;
        private GameObject _selectedSlot;

        /// <summary>
        /// Slot이 선택되었을 때 호출되는 이벤트
        /// <para> 선택된 것이 없을 경우 null을 인자로 호출한다. </para>
        /// </summary>
        public System.Action<InventorySlot> OnSlotSelected;
        
        #endregion Variables

        #region Properties
        
        /// <summary>
        /// 현재 선택된 슬롯의 InventorySlot 객체를 반환. 선택된 슬롯이 없으면 null을 반환한다.
        /// </summary>
        public InventorySlot SelectedSlot => _selectedSlot ? slotUIs[_selectedSlot] : null;
        /// <summary>
        /// 현재 UI의 모든 슬롯 개수를 반환. 비활성화된 슬롯도 포함한다.
        /// </summary>
        public int SlotCount => _inventoryScroll.content.transform.childCount;

        #endregion Properties

        #region UnityMethods
        
        private void Awake()
        {
            Assert.IsNotNull(_inventoryScroll, "Content is not assigned in the inspector");
            Assert.IsNotNull(_slotPrefab, "Slot prefab is not assigned in the inspector");
        }

        /// <summary>
        /// 비활성화되면 초기화한다. 
        /// </summary>
        private void OnDisable()
        {
            _lastSlotIndex = 0;
            DestroyAllSlots();
        }

        #endregion UnityMethods

        #region Slots

        public void CreateSlots(List<InventorySlot> slotList)
        {
            // slotList.Sort((slotA, slotB) => ItemSort.CompareByID(slotA.item, slotB.item));
            Transform parent = _inventoryScroll.content.transform;
            foreach(InventorySlot slot in slotList)
            {
                GameObject slotGo = CreateSlot(parent);
                slotGo.name += $": {_lastSlotIndex++}";

                slot.SlotUI = slotGo;
                slot.OnPostUpdate += OnPostUpdate;

                slotUIs.Add(slotGo, slot);
                slot.UpdateSlot(slot.item, slot.amount);
            }

            OrderSlot();
            ApplyFilter();
        }

        private GameObject CreateSlot(Transform parent)
        {
            GameObject slotGo = Instantiate(_slotPrefab, parent);

            AddEvent(slotGo, EventTriggerType.PointerEnter, delegate {OnEnter(slotGo);});
            AddEvent(slotGo, EventTriggerType.PointerExit, delegate {OnExit(slotGo);});
            AddEvent(slotGo, EventTriggerType.PointerClick, (data) => {OnClick(slotGo, (PointerEventData)data);} );
            AddEvent(slotGo, EventTriggerType.Scroll, (data) => {OnScroll(slotGo, (PointerEventData)data);} );
            AddEvent(slotGo, EventTriggerType.BeginDrag, (data) => {OnBeginDrag(slotGo, (PointerEventData)data);} );
            AddEvent(slotGo, EventTriggerType.Drag, (data) => {OnDrag(slotGo, (PointerEventData)data);} );
            AddEvent(slotGo, EventTriggerType.EndDrag, (data) => {OnEndDrag(slotGo, (PointerEventData)data);} );

            return slotGo;
        }

        public void RemoveSlots(List<InventorySlot> slotList)
        {
            foreach(InventorySlot slot in slotList)
            {
                DestroySlot(slot);
            }
        }

        protected void DestroyAllSlots()
        {
            // key : SlotGo, value : InventorySlot
            // 주의 : 파괴 중에는 다른 클래스에서 slot dictionary에 접근하지 말 것.
            foreach(var (slotGo, slot) in slotUIs)
            {
                slot.OnPostUpdate -= OnPostUpdate;
                slot.SlotUI = null;
                Destroy(slotGo);
            }

            slotUIs.Clear();
        }

        private void DestroySlot(InventorySlot slot)
        {
            if(!slotUIs.Remove(slot.SlotUI))
                return;
            if(slot.SlotUI == _selectedSlot)
            {
                _selectedSlot = null;
                OnSlotSelected?.Invoke(null);
            }
            Destroy(slot.SlotUI);

            slot.OnPostUpdate -= OnPostUpdate;
            slot.SlotUI = null;
        }

        /// <summary>
        /// Slot이 업데이트가 되었을 때 Slot UI를 업데이트한다.
        /// </summary>
        /// <param name="slot">업데이트 되는 Slot</param>
        private void OnPostUpdate(InventorySlot slot)
        {
            if(slot == null || slot.SlotUI == null || slot.amount <= 0)
                return;

            ItemObject itemObject = DataManager.ItemDatabase.GetItemObject(slot.item.id);
            if(itemObject == null)
            {
                Debug.LogError($"ItemObject is null. id : {slot.item.id}");
                return;
            }

            if(slot.SlotUI.TryGetComponent<SlotUI>(out var slotUi))
            {
                slotUi.IconImage = itemObject.icon;
                slotUi.ItemDescText = itemObject.type.IsEquipable() ? itemObject.subType.ToText() : slot.amount.ToString();
            }
            else
            {
                Debug.LogWarning($"Slot UI is missing");
            }
        }

        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            if(go.TryGetComponent<EventTrigger>(out var trigger) == false)
            {
                trigger = go.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry eventTrigger = new() { eventID = type };   
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        #endregion Slots

        #region Slot UI Event Callback
        
        public void OnEnter(GameObject go)
        {

        }

        public void OnExit(GameObject go)
        {
            
        }

        public void OnClick(GameObject go, PointerEventData data)
        {
            Debug.Log($"Sibling index : {go.transform.GetSiblingIndex()}");
            if(slotUIs.ContainsKey(go) == false)
            {
                Debug.LogError($"Invalid GameObject");
                return;
            }

            if(data.button == PointerEventData.InputButton.Left)
            {
                SelectSlot(go);
            }
        }   

        // Slot 위에서 Scroll, Drag를 해도 ScrollRect가 동작하도록 한다.
        public void OnScroll(GameObject go, PointerEventData data)
        {
            _inventoryScroll.OnScroll(data);
        }

        public void OnBeginDrag(GameObject go, PointerEventData data)
        {
            _inventoryScroll.OnBeginDrag(data);
        }

        public void OnDrag(GameObject go, PointerEventData data)
        {
            _inventoryScroll.OnDrag(data);
        }

        public void OnEndDrag(GameObject go, PointerEventData data)
        {
            _inventoryScroll.OnEndDrag(data);
        }

        #endregion Slot UI Event Callback

        #region public Methods

        /// <summary>
        /// 첫번째 Slot의 GameObject를 반환
        /// </summary>
        /// <returns>만약 없을 경우 null을 반환 </returns>
        public GameObject GetFirstSlotGo()
        {
            if(slotUIs.Count == 0)
                return null;

            // Linq가 순서를 보장하는지 찾지 못했다.
            // 만약에 순서가 보장되지 않는다면 for loop로 변경할 것.
            var slot = _inventoryScroll.content.Cast<Transform>()
                            .Select(child => child.gameObject)
                            .FirstOrDefault(slogGo => slogGo.activeSelf);
            return slot;
        }

        public InventorySlot GetSlotByGo(GameObject slotGo) => slotUIs.GetValueOrDefault(slotGo);

        /// <summary>
        /// 해당 슬롯을 선택한다.
        /// </summary>
        /// <param name="selectedGo">null일 경우 아무것도 선택하지 않는다.</param>
        public void SelectSlot(GameObject selectedGo)
        {
            if(_selectedSlot == selectedGo
                || (selectedGo != null && slotUIs.ContainsKey(selectedGo) == false))
            {
                return;
            }

            // Deselect current slot
            if(_selectedSlot != null)
            {
                _selectedSlot.GetComponent<TweenButton>().Deselect();
            }

            // Select current slot
            _selectedSlot = selectedGo;
            if(_selectedSlot != null)
            {
                _selectedSlot.GetComponent<TweenButton>().Select();
            }
            OnSlotSelected?.Invoke(SelectedSlot);
        }

        public GameObject GetSlotByIndex(int index)
        {
            if(index < 0 || index > _inventoryScroll.content.childCount)
                return null;
            return _inventoryScroll.content.transform.GetChild(index).gameObject;
        }

        public bool IsValidSlot(GameObject slotUI)
        {
            if(slotUI == null || slotUIs.ContainsKey(slotUI) == false)
            {
                return false;
            }
            if(_allowedType.Contains(ItemType.None) || _allowedType.Count ==0)
            {
                return true;
            }

            InventorySlot slot = slotUIs[slotUI];
            ItemObject item = DataManager.ItemDatabase.GetItemObject(slot.item.id);
            return _allowedType.Contains(item.type);
        }

        public void SetFilter(ItemType type)
        {
            if(_allowedType.Contains(type))
            {   
                return;
            }

            _allowedType.Add(type);
            ApplyFilter();
        }

        public void RemoveFilter(ItemType type)
        {
            _allowedType.Remove(type);
            ApplyFilter();
        }

        public void RemoveFilter()
        {
            _allowedType.Clear();
            _allowedType.Add(ItemType.None);
            ApplyFilter();
        }

        #endregion Public Methods

        #region Filter Slot
        
        /// <summary>
        /// Item ID에 따라 정렬
        /// </summary>
        private void OrderSlot()
        {
            var slots = new List<InventorySlot>(slotUIs.Values);
            slots.Sort((slotA, slotB) => ItemSort.CompareByID(slotA.item, slotB.item));
            for(int i = 0; i < slots.Count; ++i)
            {
                slots[i].SlotUI.transform.SetSiblingIndex(i);
            }
        }

        // First version : 효율성은 생각하지 않는다.
        // @Refactor: 최적화
        // 개선 방안 1.
        // Dirty Flag를 사용하여 Filter를 한 번만 적용하도록 할 수 있다.
        // 이럴 경우 Filter된 Slot을 바로 선택하는 Inventory UI에 주의할 것
        private void ApplyFilter()
        {
            if(_allowedType.Contains(ItemType.None) || _allowedType.Count == 0)
            {
                return;
            }

            foreach(var slot in slotUIs.Values)
            {
                bool isActive = IsFilteredSlot(slot);
                slot.SlotUI.SetActive(isActive);
            }
        }

        /// <summary>
        /// 해당 슬롯이 허용된 타입인지 확인.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        private bool IsFilteredSlot(InventorySlot slot)
        {
            ItemObject item = DataManager.ItemDatabase.GetItemObject(slot.item.id);
            return _allowedType.Contains(item.type);
        }
        
        #endregion Filter Slot
    }
}

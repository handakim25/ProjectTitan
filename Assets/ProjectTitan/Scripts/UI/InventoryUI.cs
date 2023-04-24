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
    public class InventoryUI : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] GameObject _slotPrefab;

        [SerializeField] ScrollRect _inventoryScroll;
        // None : Show all
        [SerializeField] List<ItemType> _allowedType = new List<ItemType>();

        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();
        private int _lastSlotIndex;
        private GameObject _selectedSlot;

        /// <summary>
        /// Argument : If nothing is selected, slot is null.
        /// </summary>
        public System.Action<InventorySlot> OnSlotSelected;
        
        #endregion Variables

        #region Properties
        
        public InventorySlot SelectedSlot => _selectedSlot ? slotUIs[_selectedSlot] : null;
        public int SlotCount => _inventoryScroll.content.transform.childCount;

        #endregion Properties

        #region UnityMethods
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Expect value is not null
            Assert.IsNotNull(_inventoryScroll, "Content is not assigned in the inspector");
            Assert.IsNotNull(_slotPrefab, "Slot prefab is not assigned in the inspector");
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            _lastSlotIndex = 0;
            DestroySlots();
        }

        #endregion UnityMethods

        #region Methods

        public void CreateSlots(List<InventorySlot> slotList)
        {
            // slotList.Sort((slotA, slotB) => ItemSort.CompareByID(slotA.item, slotB.item));
            Transform parent = _inventoryScroll.content.transform;
            foreach(InventorySlot slot in slotList)
            {
                GameObject slotGo = CreateSlot(parent);

                slot.SlotUI = slotGo;
                slot.OnPostUpdate += OnPostUpdate;

                slotUIs.Add(slotGo, slot);
                slotGo.name += $": {_lastSlotIndex++}";
                slot.UpdateSlot(slot.item, slot.amount);
            }
            OrderSlot();
            ApplyFilter();
        }

        // First version : 효율성은 생각하지 않는다.
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

        private void OrderSlot()
        {
            var slots = new List<InventorySlot>(slotUIs.Values);
            slots.Sort((slotA, slotB) => ItemSort.CompareByID(slotA.item, slotB.item));
            for(int i = 0; i < slots.Count; ++i)
            {
                slots[i].SlotUI.transform.SetSiblingIndex(i);
            }
        }

        private void OnPostUpdate(InventorySlot slot)
        {
            if(slot == null || slot.SlotUI == null || slot.amount <= 0)
                return;

            ItemObject itemObject = ItemDatabase.GetItemObject(slot.item.id);

            if(slot.SlotUI.TryGetComponent<SlotUI>(out var slotUi))
            {
                slotUi.IconImage = itemObject.icon;
            }
            else
            {
                Debug.LogWarning($"Slot UI is missing");
            }
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
                Debug.Log($"Destroy slot");
                DestroySlot(slot);
            }
        }

        protected void DestroySlots()
        {
            // key : SlotGo, value : InventorySlot
            // Caution: Do not access slot dictionary from other classes during destruction
            foreach(var pair in slotUIs)
            {
                var slot = pair.Value;
                slot.OnPostUpdate -= OnPostUpdate;
                slot.SlotUI = null;

                var slotGo = pair.Key;
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

        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if(trigger == null)
            {
                Debug.LogWarning($"No Event trigger component found!");
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };   
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        #endregion Methods

        #region Methods for Slot UI
        
        public void OnEnter(GameObject go)
        {

        }

        public void OnExit(GameObject go)
        {
            
        }

        public void OnClick(GameObject go, PointerEventData data)
        {
            Debug.Log($"Sibling index : {go.transform.GetSiblingIndex()}");
            InventorySlot slot = slotUIs[go];
            if(slot == null)
            {
                Debug.LogWarning($"Missing slot UI");
                return;
            }

            if(data.button == PointerEventData.InputButton.Left)
            {
                SelectSlot(go);
            }
        }   

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

        #endregion Methods for Slot UI

        #region public Methods

        /// <summary>
        /// Get first slot Game object
        /// </summary>
        /// <returns>First slot Object. If there is no slots, return null. </returns>
        public GameObject GetFirstSlotGo()
        {
            if(slotUIs.Count == 0)
                return null;

            // I cannot find that linq gaurantees the order
            // If there is a problem on order, change to for loop.
            var slot = _inventoryScroll.content.Cast<Transform>()
                            .Select(child => child.gameObject)
                            .FirstOrDefault(slogGo => slogGo.activeSelf);
            return slot;
        }

        public InventorySlot GetSlotByGo(GameObject slotGo) => slotUIs.GetValueOrDefault(slotGo);

        public void SelectSlot(GameObject selectedGo)
        {
            if(selectedGo == null)
            {
                _selectedSlot = null;
                OnSlotSelected?.Invoke(null);
                return;
            }
            if(!slotUIs.ContainsKey(selectedGo))
                return;

            _selectedSlot = selectedGo;
            OnSlotSelected?.Invoke(slotUIs[selectedGo]);
        }

        public GameObject GetSlotByIndex(int index)
        {
            if(index < 0 || index > _inventoryScroll.content.childCount)
                return null;
            return _inventoryScroll.content.transform.GetChild(index).gameObject;
        }

        public bool IsValidSlot(GameObject slotUI)
        {
            if(slotUI == null)
            {
                return false;
            }
            if(!slotUIs.ContainsKey(slotUI))
            {
                return false;
            }

            InventorySlot slot = slotUIs[slotUI];
            ItemObject item = ItemDatabase.GetItemObject(slot.item.id);
            if(_allowedType.Contains(ItemType.None) || _allowedType.Count ==0)
            {
                return true;
            }
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

        private bool IsFilteredSlot(InventorySlot slot)
        {
            ItemObject item = ItemDatabase.GetItemObject(slot.item.id);
            return _allowedType.Contains(item.type);
        }

        #endregion Methods
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using Titan.InventorySystem;

namespace Titan.UI.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] GameObject _slotPrefab;

        [SerializeField] ScrollRect _inventoryScroll;

        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();

        private int _lastSlotIndex;

        private GameObject _selectedSlot;
        /// <summary>
        /// Argument : If nothing is selected, it could be null.
        /// </summary>
        public System.Action<InventorySlot> OnSlotSelected;
        
        #endregion Variables

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
            Transform parent = _inventoryScroll.content.transform;
            foreach(InventorySlot slot in slotList)
            {
                GameObject slotGo = CreateSlot(parent);

                slot.SlotUI = slotGo;
                slotUIs.Add(slotGo, slot);
                slotGo.name += $": {_lastSlotIndex++}";
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

        protected void DestroySlots()
        {
            // key : SlotGo, value : InventorySlot
            // Caution: Do not access slot dictionary from other classes during destruction
            foreach(var pair in slotUIs)
            {
                var slot = pair.Value;
                // slot.OnPostUpdate -= 
                slot.SlotUI = null;

                var slotGo = pair.Key;
                Destroy(slotGo);
            }

            slotUIs.Clear();
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
            InventorySlot slot = slotUIs[go];
            if(slot == null)
            {
                Debug.LogWarning($"Missing slot UI");
                return;
            }

            if(data.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick(slot, data);
            }
        }   

        protected void OnLeftClick(InventorySlot slot, PointerEventData data)
        {
            OnSlotSelected?.Invoke(slot);
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

        #region Methods

        public GameObject GetFirstSlotGo()
        {
            if(slotUIs.Count == 0)
                return null;
            return _inventoryScroll.content.GetChild(0).gameObject;
        }

        public InventorySlot GetSlotByGo(GameObject slotGo) => slotUIs.GetValueOrDefault(slotGo);
        
        public void SelectSlot(GameObject selectedGo)
        {
            InventorySlot slot = slotUIs[selectedGo];
            OnSlotSelected?.Invoke(slot);
        }

        #endregion Methods
    }
}

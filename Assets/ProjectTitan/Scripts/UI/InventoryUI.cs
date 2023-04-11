using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Titan.InventorySystem;

namespace Titan.UI.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] InventoryObject inventoryObject;
        [SerializeField] GameObject slotPrefab;

        [SerializeField] GameObject content;

        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();

        #endregion Variables

        #region UnityMethods
        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Expect value is not null
            Assert.IsNotNull(inventoryObject, "Inventory is not assigned in the inspector");
            Assert.IsNotNull(content, "Content is not assigned in the inspector");
            Assert.IsNotNull(slotPrefab, "Slot prefab is not assigned in the inspector");
        }
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            CreateSlots();    
        }

        #endregion UnityMethods
        protected void CreateSlots()
        {
            slotUIs.Clear();
            var slots = inventoryObject.Slots;

            Transform parent = content.transform;
            for(int i = 0; i < slots.Count; ++i)
            {
                // Caution : Directly accessing the transform of UI elements is risky.
                // It can lead to unintended results.
                GameObject slotGo = Instantiate(slotPrefab, parent);

                slotUIs.Add(slotGo, slots[i]);
                slotGo.name += $": {i}";
            }
        }

        protected void OnInventoryChangedHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}

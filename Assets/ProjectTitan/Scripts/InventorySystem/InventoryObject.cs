using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Varialbes
        [SerializeField] protected ItemDatabase itemDatabase;
        // @ToDo : Create custom eidtor for Inventory.
        [SerializeField] Inventory inventory;
        // [SerializeField] 

        // @refactor
        // Pass changed arguemnts.
        // So that, it does not need to update all slots.
        public event System.Action OnInventoryChanged;

        #endregion Varialbes

        #region Properties
        
        public List<InventorySlot> Slots => inventory.Slots;
        #endregion Properties

        #region Methods
        
        public bool AddItem(Item item, int amount)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(Item item, int amount)
        {
            throw new System.NotImplementedException();
        }

        #endregion Methods
#if UNITY_EDITOR
        #region TestMethods

        public void AddRandomItem()
        {
            if(itemDatabase.itemObjects.Length == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, itemDatabase.itemObjects.Length);
        }

        #endregion TestMethods
#endif
    }
}

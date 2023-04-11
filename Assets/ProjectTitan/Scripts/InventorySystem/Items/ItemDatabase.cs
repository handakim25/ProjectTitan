using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemObject[] itemObjects;

        // @Refactor
        // Refactoring to avoid potential issues with differing ids
        // during saving and loading process.
        public void OnValidate()
        {
            for(int i = 0; i < itemObjects.Length; ++i)
            {
                itemObjects[i].data.id = i;
            }
        }
    }
}

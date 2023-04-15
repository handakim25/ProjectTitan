using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.InventorySystem.Items
{
    public sealed class ItemDatabase : MonoSingleton<ItemDatabase>
    {
        [SerializeField] private ItemDatabaseObject itemDatabase;
        
        static public ItemObject GetItemObject(int id)
        {
            if(id < 0 || id > Instance.itemDatabase.itemObjects.Length)
                return null;
            return Instance.itemDatabase.itemObjects[id];
        }

        static public int Length => Instance.itemDatabase.itemObjects.Length;
    }
}

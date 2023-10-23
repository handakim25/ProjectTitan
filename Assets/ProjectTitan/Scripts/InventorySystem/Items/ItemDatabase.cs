using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using Titan.Resource;

namespace Titan.InventorySystem.Items
{
    public sealed class ItemDatabase : BaseData
    {
        public ItemDatabaseObject itemDatabase;
        
        public ItemObject GetItemObject(int id)
        {
            if(id < 0 || id > itemDatabase.itemObjects.Length)
                return null;
            return itemDatabase.itemObjects[id];
        }

        public int Length => itemDatabase.itemObjects.Length;
    }
}

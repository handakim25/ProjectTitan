using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject
    {
        public ItemObject[] itemObjects;

        public string[] GetAllNames()
        {
            return itemObjects.Select(item => item.ItemName).ToArray();
        }

#region EditorCode
#if UNITY_EDITOR
        public void OnValidate()
        {
            GetAllItemsInDatabase();
        }

        [ContextMenu("Get All Items")]
        private void GetAllItemsInDatabase()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(ItemObject)}");
            itemObjects = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<ItemObject>(path))
                .Where(asset => asset != null)
                .ToArray();

            for(int i = 0; i < itemObjects.Length; ++i)
            {
                itemObjects[i].data.id = i;
            }
        }
#endif
#endregion EditorCode
    }
}

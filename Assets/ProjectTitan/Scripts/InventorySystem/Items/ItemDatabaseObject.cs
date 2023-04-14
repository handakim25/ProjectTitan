using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
#if UNITY_EDITOR
        using UnityEditor;
#endif

    // @refacor
    // Change to singleton class
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject
    {
        public ItemObject[] itemObjects;

#region EditorCode
#if UNITY_EDITOR
        // @Refactor
        // Refactoring to avoid potential issues with differing ids
        // during saving and loading process.
        public void OnValidate()
        {
            List<ItemObject> assetList = new List<ItemObject>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(ItemObject)}");
            foreach(var guid in guids)
            {   
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ItemObject asset = AssetDatabase.LoadAssetAtPath<ItemObject>(assetPath);
                if(asset != null)
                {
                    assetList.Add(asset);
                }
            }

            for(int i = 0; i < assetList.Count; ++i)
            {
                assetList[i].data.id = i;
            }
            itemObjects = assetList.ToArray();
        }
#endif
#endregion EditorCode
    }
}

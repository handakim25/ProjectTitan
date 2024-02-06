using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Titan.InventorySystem.Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory System/Items/New Item")]
    public class ItemObject : ScriptableObject
    {
        public string ItemName;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType type = ItemType.None;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemSubType subType = ItemSubType.None;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemRarity rarity = ItemRarity.Common;
        public bool stackable;

        [Tooltip("인벤토리 UI에서 보여질 아이콘 이미지")]
        [SpritePreview]
        [JsonIgnore]
        public Sprite icon;

        [TextArea(5,20)]
        [Tooltip("아이템 창에서 보여질 설명")]
        public string description;

        public Item data = new();

        [ContextMenu("Show Json Data")]
        public void ShowJsonData()
        {
            // Debug.Log(JsonUtility.ToJson(this, true));
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Debug.Log(json);
        }
    }
}
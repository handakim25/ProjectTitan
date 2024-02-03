using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    [SerializeField]
    public enum ItemType
    {
        None,
        Weapon,
        Core,
        Food,
        CrafitingMaterial,
        QuestItem,
    }

    public static class ItemTypeExtension
    {
        public static string ToText(this ItemType type)
        {
            return type switch
            {
                ItemType.Weapon => "무기",
                ItemType.Core => "코어",
                ItemType.Food => "음식",
                ItemType.CrafitingMaterial => "제작 재료",
                ItemType.QuestItem => "퀘스트 아이템",
                _ => "없음",
            };
        }
    }
}

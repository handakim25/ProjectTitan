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
        Acc,
        Consumable,
        Food,
        QuestItem,
        Misc,
    }

    public static class ItemTypeExtension
    {
        public static string ToText(this ItemType type)
        {
            return type switch
            {
                ItemType.Weapon => "무기",
                ItemType.Acc => "장신구",
                ItemType.Consumable => "소모품",
                ItemType.Food => "음식",
                ItemType.QuestItem => "퀘스트 아이템",
                ItemType.Misc => "기타",
                _ => "없음",
            };
        }
    }
}

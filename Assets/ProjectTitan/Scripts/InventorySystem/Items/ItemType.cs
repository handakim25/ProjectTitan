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

    public enum ItemSubType
    {
        None,
        // Weapon
        Weapon_Sword,
        Weapon_Spear,
        // Acc
        Acc_Ring,
        Acc_Necklace,
        // Consumable
        Consumable_Potion,
        // Food
        Food_Meal,
        // Misc
        Misc_Material,
        Misc_Trinket,
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

        public static string ToText(this ItemSubType type)
        {
            return type switch
            {
                ItemSubType.Weapon_Sword => "검",
                ItemSubType.Weapon_Spear => "창",
                ItemSubType.Acc_Ring => "반지",
                ItemSubType.Acc_Necklace => "목걸이",
                ItemSubType.Consumable_Potion => "물약",
                ItemSubType.Food_Meal => "음식",
                ItemSubType.Misc_Material => "재료",
                ItemSubType.Misc_Trinket => "잡동사니",
                _ => "없음",
            };
        }
    }
}

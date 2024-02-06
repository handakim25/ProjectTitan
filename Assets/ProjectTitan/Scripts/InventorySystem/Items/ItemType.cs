using System;
using System.Linq;
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
        [ItemSubCategoryOf(ItemType.Weapon)]
        Sword,
        [ItemSubCategoryOf(ItemType.Weapon)]
        Spear,
        // Acc
        [ItemSubCategoryOf(ItemType.Acc)]
        Ring,
        [ItemSubCategoryOf(ItemType.Acc)]
        Necklace,
        // Consumable
        [ItemSubCategoryOf(ItemType.Consumable)]
        Potion,
        // Food
        [ItemSubCategoryOf(ItemType.Food)]
        Meal,
        // Misc
        [ItemSubCategoryOf(ItemType.Misc)]
        Material,
        [ItemSubCategoryOf(ItemType.Misc)]
        Trinket,
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
                ItemSubType.Sword => "검",
                ItemSubType.Spear => "창",
                ItemSubType.Ring => "반지",
                ItemSubType.Necklace => "목걸이",
                ItemSubType.Potion => "물약",
                ItemSubType.Meal => "음식",
                ItemSubType.Material => "재료",
                ItemSubType.Trinket => "잡동사니",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// ItemSubType이 ItemType과 같은지 확인
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubTypeOf(this ItemSubType subType, ItemType type)
        {
            return subType.GetItemType() == type;
        }

        /// <summary>
        /// ItemSubType이 속한 ItemType을 반환
        /// </summary>
        /// <param name="subType"></param>
        /// <returns>만약 속한 ItemType이 없다면 ItemType.None을 반환</returns>
        public static ItemType GetItemType(this ItemSubType subType)
        {
            // @Fix
            // 이 방식은 ItemSubType에 수식된 Attribute를 구하는 방식이다.
            // Type subTypeInfo = subType.GetType();

            // Get member info for the enum value
            var memberInfo = typeof(ItemSubType).GetMember(subType.ToString())
                .First();
            var subTypeAttribute = (ItemSubCategoryOf)Attribute.GetCustomAttribute(memberInfo, typeof(ItemSubCategoryOf), false);
            // 속한 Category가 존재하지 않은 경우
            if(subTypeAttribute == null)
            {
                return ItemType.None;
            }
            return subTypeAttribute.Type;
        }

        /// <summary>
        /// ItemType에 속한 ItemSubType들을 반환
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<ItemSubType> GetSubTypes(this ItemType type)
        {
            var values = Enum.GetValues(typeof(ItemSubType)).Cast<ItemSubType>();
            return values.Where(subType => subType.GetItemType() == type);
        }
    }

    // see https://stackoverflow.com/questions/26472062/mapping-enum-to-sub-enum
    // see https://stackoverflow.com/questions/24823061/get-all-types-under-each-category-using-enum-in-c-sharp
    [AttributeUsage(AttributeTargets.Field)]
    public class ItemSubCategoryOf : Attribute
    {
        public ItemType Type {get; private set;}
        public ItemSubCategoryOf(ItemType type)
        {
            Type = type;
        }
    }
}

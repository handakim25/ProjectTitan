using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Titan.InventorySystem.Items;
using UnityEditor;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueItemNodeView : DialogueConditionNodeView
    {
        /// <summary>
        /// ItemSO의 GUID
        /// </summary>
        public string ItemGUID;
        /// <summary>
        /// Item ID
        /// </summary>
        public string ItemID;
        /// <summary>
        /// Item 개수
        /// </summary>
        public int ItemCount;
        private int _maxItemCount = 999;
        
        protected override void BuildView()
        {
            base.BuildView();

            var customContainer = new VisualElement();

            var foldout = new Foldout() {text = "Item Require", value = true};
            var itemField = new ObjectField("Item Object")
            {
                objectType = typeof(ItemObject)
            };
            if(string.IsNullOrEmpty(ItemGUID) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(ItemGUID);
                var item = AssetDatabase.LoadAssetAtPath<ItemObject>(assetPath);
                itemField.SetValueWithoutNotify(item);
                ItemID = item.data.id.ToString();
            }
            itemField.RegisterValueChangedCallback(evt =>
            {
                var item = evt.newValue as ItemObject;
                if(item != null)
                {
                    ItemID = item.data.id.ToString();
                    var assetPath = AssetDatabase.GetAssetPath(item);
                    ItemGUID = AssetDatabase.AssetPathToGUID(assetPath);
                }
                else
                {
                    ItemID = null;
                    ItemGUID = null;
                }
            });

            var itemCountField = new IntegerField("Item Count");
            itemCountField.SetValueWithoutNotify(ItemCount);
            itemCountField.RegisterValueChangedCallback(
                evt => {
                    ItemCount = Mathf.Clamp(evt.newValue, 0, _maxItemCount);
                    itemCountField.SetValueWithoutNotify(ItemCount);
                }
            );

            foldout.Add(itemField);
            foldout.Add(itemCountField);
            customContainer.Add(foldout);
            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }

        public override Requirement GetRequirement()
        {
            return new Requirement()
            {
                Type = Requirement.RequirementType.Item,
                TargetID = ItemID,
                TargetCount = ItemCount,
            };
        }
    }
}

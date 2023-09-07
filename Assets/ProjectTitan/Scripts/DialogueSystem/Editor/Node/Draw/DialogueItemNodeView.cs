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
        protected override void BuildView()
        {
            base.BuildView();

            var customContainer = new VisualElement();

            var foldout = new Foldout() {text = "Item Require", value = true};
            var objectField = new ObjectField("test")
            {
                objectType = typeof(ItemObject)
            };
            objectField.RegisterValueChangedCallback(evt =>
            {
                Debug.Log(evt.newValue);
                Debug.Log(evt.newValue.GetType());
                Debug.Log($"ItemObject : {evt.newValue as ItemObject}");
                Debug.Log($"Binding path : {objectField.bindingPath}");
                Debug.Log($"Value : {objectField.value}");
                Debug.Log($"Asset Path : {AssetDatabase.GetAssetPath(objectField.value)}");
                Debug.Log($"GUID : {AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objectField.value))}");
            });

            
            foldout.Add(objectField);
            customContainer.Add(foldout);
            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }
    }
}

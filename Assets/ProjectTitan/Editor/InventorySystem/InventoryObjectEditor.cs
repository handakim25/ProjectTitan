using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Titan.InventorySystem.Items
{
    [CustomEditor(typeof(InventoryObject))]
    public class InventoryObjectEditor : Editor
    {
        private InventoryObject _inventoryObject;
        private ItemDatabaseObject _itemDatabase;

        private void OnEnable()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemDatabaseObject)}");
            _itemDatabase = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<ItemDatabaseObject>(path))
                .FirstOrDefault();

            _inventoryObject = target as InventoryObject;
        }

        private int _itemSelectionIndex = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if(_itemDatabase == null)
            {
                EditorGUILayout.HelpBox("Item Database is not set", MessageType.Warning);
                return;
            }

            if(GUILayout.Button("Add Random Item"))
            {
                int randomItemIndex = UnityEngine.Random.Range(0, _itemDatabase.itemObjects.Length);
                AddItem(randomItemIndex);
            }

            EditorGUILayout.Space();

            _itemSelectionIndex = EditorGUILayout.Popup("Item to Add", _itemSelectionIndex, _itemDatabase.GetAllNames());
            if(GUILayout.Button("Add Selected Item"))
            {
                AddItem(_itemSelectionIndex);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddItem(int index)
        {
            if(_itemDatabase == null || index > _itemDatabase.itemObjects.Length)
            {
                return;
            }

            var itemObject = _itemDatabase.itemObjects[index];
            _inventoryObject.AddItem(itemObject, 1);

            EditorUtility.SetDirty(_inventoryObject);
        }
    }
}

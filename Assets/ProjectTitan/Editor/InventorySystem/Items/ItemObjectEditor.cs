using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.InventorySystem.Items
{
    [CustomEditor(typeof(ItemObject))]    
    public class ItemObjectEditor : Editor
    {
        SerializedProperty itemName;
        SerializedProperty type;
        SerializedProperty subType;
        SerializedProperty rarity;
        SerializedProperty stackable;
        SerializedProperty icon;
        SerializedProperty description;
        SerializedProperty data;

        GUIContent subTypeLabel;

        private void OnEnable()
        {
            itemName = serializedObject.FindProperty("ItemName");
            type = serializedObject.FindProperty("type");
            subType = serializedObject.FindProperty("subType");
            subTypeLabel = new GUIContent(subType.displayName);
            rarity = serializedObject.FindProperty("rarity");
            stackable = serializedObject.FindProperty("stackable");
            icon = serializedObject.FindProperty("icon");
            description = serializedObject.FindProperty("description");
            data = serializedObject.FindProperty("data");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("ID", data.FindPropertyRelative("id").intValue.ToString());
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(itemName);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(type);
            if(EditorGUI.EndChangeCheck())
            {
                ItemType curType = (ItemType)type.enumValueIndex;
                subType.enumValueIndex = (int)curType.GetSubTypes().FirstOrDefault();
            }
            subType.enumValueIndex = (int)(ItemSubType)EditorGUILayout.EnumPopup(subTypeLabel,
                                                                                 (ItemSubType)subType.enumValueIndex,
                                                                                 FilterSubType,
                                                                                 false);

            EditorGUILayout.PropertyField(rarity);
            EditorGUILayout.PropertyField(stackable);
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(description);

            serializedObject.ApplyModifiedProperties();
        }

        private bool FilterSubType(System.Enum filterSubType)
        {
            ItemType curItemType = (ItemType)type.enumValueIndex;
            ItemSubType curSubType = (ItemSubType)filterSubType;

            return curSubType.IsSubTypeOf(curItemType);
        }
    }
}

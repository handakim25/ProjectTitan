using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer  : PropertyDrawer
    {
        private const float _previewWidth = 64;
        private const float _previewHeight = 64;
        private const float _bottomMargin = 4;

        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // See https://docs.unity3d.com/kr/current/Manual/editor-PropertyDrawers.html
            // https://docs.unity3d.com/ScriptReference/EditorGUI.ObjectField.html
            if(!IsSprite(property))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Sprite), false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // if the property is not a Sprite, use the default height
            // See https://forum.unity.com/threads/drawing-a-sprite-in-editor-window.419199/
            if (!IsSprite(property))
            {
                return base.GetPropertyHeight(property, label);
            }
            return EditorGUIUtility.singleLineHeight > _previewHeight + _bottomMargin ?
                EditorGUIUtility.singleLineHeight : _previewHeight + _bottomMargin;
        }

        private bool IsSprite(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Sprite;
        }
    }
}

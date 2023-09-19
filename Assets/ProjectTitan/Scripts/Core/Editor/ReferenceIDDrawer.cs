using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan
{
    [CustomPropertyDrawer(typeof(ReferenceID<>))]
    public class ReferenceIDDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var idField = new Rect(position.x, position.y, 300, position.height);
            var objectField = new Rect(position.x + 305, position.y, position.width - 305, position.height);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(idField, property.FindPropertyRelative("ID"), label);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(objectField, property.FindPropertyRelative("reference"), GUIContent.none);
            if(EditorGUI.EndChangeCheck())
            {
                var obj = property.FindPropertyRelative("reference").objectReferenceValue;
                if(obj != null && obj is IRefereceable refereceable)
                {
                    property.FindPropertyRelative("ID").stringValue = refereceable.ID;
                }
                else
                {
                    property.FindPropertyRelative("ID").stringValue = "";
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}

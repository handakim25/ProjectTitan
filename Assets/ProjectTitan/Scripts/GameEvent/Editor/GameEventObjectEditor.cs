using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.GameEventSystem
{
    [CustomEditor(typeof(GameEventObject))]
    public class GameEventObjectEditor : UnityEditor.Editor
    {
        private GameEventObject _gameEventObject;
        private SerializedProperty _gameEventProperty;
        private SerializedProperty _eventDescriptionProperty;

        private void OnEnable()
        {
            _gameEventObject = (GameEventObject)target;
            _gameEventProperty = serializedObject.FindProperty("GameEvent");
            _eventDescriptionProperty = serializedObject.FindProperty("EventDescription");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_gameEventProperty);
            EditorGUILayout.PropertyField(_eventDescriptionProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

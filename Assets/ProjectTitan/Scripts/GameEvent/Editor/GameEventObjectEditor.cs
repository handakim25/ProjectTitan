using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Titan.Resource;

namespace Titan.GameEventSystem
{
    [CustomEditor(typeof(GameEventObject))]
    public class GameEventObjectEditor : Editor
    {
        private GameEventObject _gameEventObject;
        private SerializedProperty _gameEventProperty;
        private SerializedProperty _eventDescriptionProperty;

        private void OnEnable()
        {
            if(target == null)
                return;
            _gameEventObject = (GameEventObject)target;
            _gameEventProperty = serializedObject.FindProperty("GameEvent");
            _eventDescriptionProperty = serializedObject.FindProperty("EventDescription");
        }

        public override void OnInspectorGUI()
        {
            if(target == null)
                return;
            serializedObject.Update();

            EditorGUILayout.PropertyField(_gameEventProperty);
            EditorGUILayout.PropertyField(_eventDescriptionProperty);

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Assets/Create/Game Play/GameEvent")]
        public static void CreateGameEvent()
        {
            var gameEvent = CreateInstance<GameEventObject>();

            AssetCreator.CreateAssetInCurrentFolder<GameEventObject>("New Game Event", (gameEvent) =>
            {
                gameEvent.GameEvent.EventName = gameEvent.name;
                EditorUtility.SetDirty(gameEvent);
                AssetDatabase.SaveAssetIfDirty(gameEvent);
            });
        }
    }
}

using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Titan.GameEventSystem;
using System;

namespace Titan.Resource
{
    public class GameEventTool : EditorWindow
    {
        public int uiWidthLarge = 450;
        public int uiWidthMiddle = 300;
        public int uiWidthSmall = 200;

        Vector2 _listScrollPos = Vector2.zero;
        Vector2 _contentScrollPos = Vector2.zero;
        static int _selection = 0;
        UnityEditor.Editor _gameEventEditor = null;

        static private string _defaultFilePath = "Assets/ProjectTitan/Scripts/ScriptableObjects/";
        static private string _defaultFileName = "GameEventData";
        private static GameEventDataBuilder _gameEventData;
        private static ReorderableList _gameEventList;

        [MenuItem("Tools/Game Event Tool")]
        static void Init()
        {
            InitGameData();

            var window = GetWindow<GameEventTool>(false, "Game Event Tool");
            window.Show();
        }

        private static void InitGameData()
        {
            _gameEventData = AssetDatabase.LoadAssetAtPath<GameEventDataBuilder>(_defaultFilePath + _defaultFileName + ".asset");
            if(_gameEventData == null)
            {
                _gameEventData = CreateInstance<GameEventDataBuilder>();
                AssetDatabase.CreateAsset(_gameEventData, _defaultFilePath + _defaultFileName + ".asset");
                AssetDatabase.SaveAssets();
            }
            _gameEventList = new ReorderableList(_gameEventData.GameEvents, typeof(GameEventObject), true, true, false, false);
            _gameEventList.drawElementCallback = (rect, index, isActive, isFocus) =>
            {
                var elemtent = _gameEventData.GameEvents[index];
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.LabelField(rect, $"{_gameEventData.GameEvents[index].GameEvent.EventName}");
            };
            _gameEventList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Game Event List");
            };

            _gameEventList.onSelectCallback = (list) =>
            {
                _selection = list.index;
            };
        }

        private void OnGUI()
        {
            if(_gameEventData == null)
            {
                InitGameData();
            }

            EditorGUILayout.BeginVertical();
            {
                // Top Layer
                EditorTopLayer();

                // Body Layer
                EditorGUILayout.BeginHorizontal();
                {
                    EditorBodyToolListLayer(uiWidthMiddle);

                    EditorBodyContentLayer();
                }
                EditorGUILayout.EndHorizontal();

                // Bottom Layer
                EditorBottomLayer();
            }
            EditorGUILayout.EndVertical();
        }

        

        private void EditorTopLayer()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Find All Unregistered Events"))
                {
                    FindAllUnregisteredEvents();
                }
                if(GUILayout.Button("ReorderIndex"))
                {
                    ReorderIndex(_gameEventData.GameEvents.Select(x => x.GameEvent).ToList());
                    EditorUtility.SetDirty(_gameEventData);
                }
                if(GUILayout.Button("Select Bulder Object"))
                {
                    Selection.activeObject = _gameEventData;
                    EditorGUIUtility.PingObject(_gameEventData);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void FindAllUnregisteredEvents()
        {
            // Find All Unregistered Events
            var assets = AssetDatabase.FindAssets("t:GameEventObject", new string[] { "Assets/ProjectTitan/Scripts/ScriptableObjects/GameEvent" });
            foreach(var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var gameEventObject = AssetDatabase.LoadAssetAtPath<GameEventObject>(path);

                if(_gameEventData.GameEvents.Any(x => x.GameEvent.EventName == gameEventObject.GameEvent.EventName) == false)
                {
                    _gameEventData.AddData(gameEventObject);
                }
            }
        }

        private void EditorBodyToolListLayer(int uiWidth)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical("box");
                {
                    _listScrollPos = EditorGUILayout.BeginScrollView(_listScrollPos);
                    {
                        _gameEventList.DoLayoutList();
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void EditorBodyContentLayer()
        {
            EditorGUILayout.BeginVertical();
            {
                _contentScrollPos = EditorGUILayout.BeginScrollView(_contentScrollPos);
                {
                    if(_gameEventData.GameEvents.Count > 0)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.Separator();
                            EditorGUILayout.Separator();
                            if(_gameEventEditor == null)
                            {
                                _gameEventEditor = UnityEditor.Editor.CreateEditor(_gameEventData.GameEvents[_selection]);
                            }
                            else
                            {
                                if(_gameEventEditor.target != _gameEventData.GameEvents[_selection])
                                {
                                    _gameEventEditor = UnityEditor.Editor.CreateEditor(_gameEventData.GameEvents[_selection]);
                                }
                            }
                            
                            if(_gameEventEditor != null)
                            {
                                _gameEventEditor.OnInspectorGUI();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void EditorBottomLayer()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Save To Json File"))
                {
                    if(!HasDuplicateName())
                    {
                        _gameEventData.SaveData();
                    }
                    else
                    {
                        Debug.LogError("Game Event Data has duplicate name");
                    }
                }
                if(GUILayout.Button("Load From Json File(Not yet implemented)"))
                {

                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ReorderIndex(List<GameEvent> gameEvents)
        {
            Debug.Log($"ReorderIndex");
            int startIndex = 0;
            foreach(var gameEvent in gameEvents)
            {
                Debug.Log("startIndex : " + startIndex);
                if(gameEvent.index > startIndex)
                {
                    startIndex = gameEvent.index;
                }
                startIndex++;
            }
        }

        private bool HasDuplicateName()
        {
            return _gameEventData.GameEvents.Count != _gameEventData.GameEvents.Select(x => x.GameEvent.EventName).Distinct().Count();
        }
    }
}

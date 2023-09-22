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
        private static GameEventData _gameEventData;
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
            _gameEventData = AssetDatabase.LoadAssetAtPath<GameEventData>(_defaultFilePath + _defaultFileName + ".asset");
            if(_gameEventData == null)
            {
                _gameEventData = CreateInstance<GameEventData>();
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

            RemoveMissingEvent(_gameEventData.GameEvents);
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
                    EditorBodyToolListLayer(uiWidthSmall);

                    EditorBodyContentLayer();
                }
                EditorGUILayout.EndHorizontal();

                // Bottom Layer
                EditorBottomLayer();
            }
            EditorGUILayout.EndVertical();
        }

        private static void RemoveMissingEvent(List<GameEventObject> gameEvents)
        {
            for (int i = gameEvents.Count - 1; i >= 0; i--)
            {
                if (gameEvents[i] == null)
                {
                    gameEvents.RemoveAt(i);
                    continue;
                }

                try
                {
                    gameEvents[i].name = gameEvents[i].name;
                }
                catch (MissingReferenceException)
                {
                    gameEvents.RemoveAt(i);
                }
            }
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
                if(GUILayout.Button("Remove Object"))
                {
                    if(_selection >= 0 && _selection < _gameEventData.GameEvents.Count)
                    {
                        _gameEventData.GameEvents.RemoveAt(_selection);
                        EditorUtility.SetDirty(_gameEventData);
                    }
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
                        AssetDatabase.Refresh();
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
            int startIndex = 0;
            foreach(var gameEvent in gameEvents)
            {
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

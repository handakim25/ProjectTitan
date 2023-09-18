using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Titan.QuestSystem;

using UnityObject = UnityEngine.Object;

namespace Titan.Resource
{
    public class QuestTool : EditorWindow
    {
        public int uiWidthLarge = 450;
        public int uiWidthMiddle = 300;
        public int uiWidthSmall = 200;

        public int selection = 0;
        private Vector2 _listScrollPos = Vector2.zero;
        private Vector2 _contentScrollPos = Vector2.zero;

        private QuestObject questSource;
        private static QuestDatabase _questDatabase;      

        private const string EnumName = "QuestList";

        [MenuItem("Tools/Quest Tool")]
        static void Init()
        {
            _questDatabase = CreateInstance<QuestDatabase>();
            _questDatabase.LoadData();

            QuestTool window = GetWindow<QuestTool>(false, "Quest Tool");
            window.Show();
        }

        private void OnGUI()
        {
            if(_questDatabase == null)
            {
                _questDatabase = CreateInstance<QuestDatabase>();
                _questDatabase.LoadData();
            }

            EditorGUILayout.BeginVertical();
            {
                // Tool Top
                UnityObject source = questSource;
                EditorHelper.EditorToolTopLayer(_questDatabase, ref selection, ref source, uiWidthMiddle);
                questSource = (QuestObject)source;

                // Tool Body
                EditorGUILayout.BeginHorizontal();
                {
                    // Tool Body - Left List
                    EditorHelper.EditorToolListLayer(_questDatabase, ref _listScrollPos, ref selection, ref source, uiWidthMiddle);
                    questSource = (QuestObject)source;

                    // Tool Body - Right Content
                    EditorToolBodyLayer();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            // Tool Bottom
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Relaod Settings"))
                {
                    _questDatabase = CreateInstance<QuestDatabase>();
                    _questDatabase.LoadData();
                    selection = 0;
                }
                if(GUILayout.Button("Save Settings"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndHorizontal();
        }  

        private UnityEditor.Editor assetEditor;

        private void EditorToolBodyLayer()
        {
            EditorGUILayout.BeginVertical();
            {
                _listScrollPos = EditorGUILayout.BeginScrollView(_listScrollPos);
                {
                    if (_questDatabase.Count > 0)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.Separator();
                            EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                            _questDatabase.names[selection] = EditorGUILayout.TextField("Name", _questDatabase.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                            var curQuest = _questDatabase.Quests[selection];
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            if(HasDuplicateName())
            {
                Debug.LogError("QuestTool: Quest name is duplicated");
                List<string> duplicates = _questDatabase.names.GroupBy(x => x)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key).ToList();
                foreach(string duplicate in duplicates)
                {
                    Debug.Log($"Duplicate : {duplicate}");
                }
                return;
            }

            _questDatabase.SaveData();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private bool HasDuplicateName()
        {
            return _questDatabase.names.Length != _questDatabase.names.Distinct().Count();
        }
    }
}

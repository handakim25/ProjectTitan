using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan
{
    public class RenameAsset : EditorWindow
    {
        // @Fix
        // EditorStyles.label을 이용해서 생성해버리면 click 상태나 focus 상태가 이미 들어가 있다.
        // 따라서 원래 검은 색상으로 돌아가버리는 문제가 생기므로 완전히 새로 생성해야 한다.
        private GUIStyle _warningStyle;
        private GUIStyle WarningStyle => _warningStyle ??= new GUIStyle() { normal = { textColor = Color.red }, fontStyle = FontStyle.Bold };

        enum TabIndex
        {
            AddPrefix,
            Replace,
        }
        private int _tabIndex = 0;
        private string[] _tabNames;

        [MenuItem("Tools/Rename Asset Util")]
        public static void ShowWindow()
        {
            GetWindow<RenameAsset>(true, "Rename Asset");
        }

        private void OnGUI()
        {
            _tabNames ??= System.Enum.GetNames(typeof(TabIndex));
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabNames);

            GUILayout.Label("Warning: Undo is not supported yet", WarningStyle);

            switch((TabIndex)_tabIndex)
            {
                case TabIndex.AddPrefix:
                    DrawAddPrefixtab();
                    break;
                case TabIndex.Replace:
                    DrawReplaceTab();
                    break;
            }
        }

        private string _renamePrefix = "";

        private void DrawAddPrefixtab()
        {
            _renamePrefix = EditorGUILayout.TextField("Prefix", _renamePrefix);

            var objects = GetSelectedAssets();

            EditorGUI.BeginDisabledGroup(objects == null || objects.Length == 0 || string.IsNullOrEmpty(_renamePrefix));
            if (GUILayout.Button("AddPrefix"))
            {
                AddPrefix(Selection.objects, _renamePrefix);
            }
            EditorGUI.EndDisabledGroup();

            if(objects == null || objects.Length == 0 )
            {
                EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            }
            if(string.IsNullOrEmpty(_renamePrefix))
            {
                EditorGUILayout.HelpBox("Prefix is empty", MessageType.Warning);
            }
        }

        private void AddPrefix(Object[] objects, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                Debug.LogError("Prefix is empty");
                return;
            }
            if (objects == null || objects.Length == 0)
            {
                Debug.LogError("No object selected");
                return;
            }

            RenameAssets(objects, name => prefix + name);
        }

        private string _replaceFrom = "";
        private string _replaceTo = "";

        private void DrawReplaceTab()
        {
            _replaceFrom = EditorGUILayout.TextField("From", _replaceFrom);
            _replaceTo = EditorGUILayout.TextField("To", _replaceTo);

            var objects = GetSelectedAssets();

            EditorGUI.BeginDisabledGroup(objects == null || objects.Length == 0 || string.IsNullOrEmpty(_replaceFrom) || string.IsNullOrEmpty(_replaceTo));
            if(GUILayout.Button("Replace"))
            {
                Replace(Selection.objects, _replaceFrom, _replaceTo);
            }
            EditorGUI.EndDisabledGroup();

            if (objects == null || objects.Length == 0)
            {
                EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            }
            if(string.IsNullOrEmpty(_replaceFrom) || string.IsNullOrEmpty(_replaceTo))
            {
                EditorGUILayout.HelpBox("From or To is empty", MessageType.Warning);
            }

            if(objects != null && objects.Length > 0 && !string.IsNullOrEmpty(_replaceFrom) && !string.IsNullOrEmpty(_replaceTo))
            {
                Object curObj = objects[0];
                string path = AssetDatabase.GetAssetPath(curObj);
                string name = GetNameFromPath(path);
                string replaced = name.Replace(_replaceFrom, _replaceTo);
                EditorGUILayout.LabelField($"Preview : {replaced}");                
            }
        }

        private void Replace(Object[] objects, string replaceFrom, string replaceTo)
        {
            if (string.IsNullOrEmpty(replaceFrom) || string.IsNullOrEmpty(replaceTo))
            {
                Debug.LogError("From or To is empty");
                return;
            }
            if (objects == null || objects.Length == 0)
            {
                Debug.LogError("No object selected");
                return;
            }

            RenameAssets(objects, name => name.Replace(replaceFrom, replaceTo));
        }

        /// <summary>
        /// 에셋 이름을 변경. nameProcessor를 통해서 이름 변경 방법을 결정
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="nameProcessor"></param>
        private void RenameAssets(Object[] objects, System.Func<string, string> nameProcessor)
        {
            var paths = objects.Select(obj => AssetDatabase.GetAssetPath(obj)).ToArray();
            for(int i = 0; i < paths.Length; i++)            
            {
                string path = paths[i];
                string name = path[(path.LastIndexOf('/') + 1)..];
                name = name[..name.LastIndexOf('.')];
                string newName = nameProcessor(name);
                string err = AssetDatabase.RenameAsset(path, newName);
                if(!string.IsNullOrEmpty(err))
                {
                    Debug.LogError(err);
                }
            }
        }

        /// <summary>
        /// 선택된 에셋을 반환. 에셋이 아닌 것은 제외
        /// </summary>
        /// <returns></returns>
        private Object[] GetSelectedAssets()
        {
            return Selection.GetFiltered<Object>(SelectionMode.Assets);
        }

        /// <summary>
        /// 경로로부터 확장자를 제외한 이름을 반환
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>이름</returns>
        private string GetNameFromPath(string path)
        {
            string name = path[(path.LastIndexOf('/') + 1)..];
            return name[..name.LastIndexOf('.')];
        }
    }
}

using System.Linq;
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
            Rename,
            Show, // For Debugging
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

            var objects = GetSelectedAssets();

            switch((TabIndex)_tabIndex)
            {
                case TabIndex.AddPrefix:
                    DrawAddPrefixtab(objects);
                    break;
                case TabIndex.Replace:
                    DrawReplaceTab(objects);
                    break;
                case TabIndex.Rename:
                    DrawRenameTab(objects);
                    break;
                case TabIndex.Show:
                    DrawShowTab(objects);
                    break;
            }
        }

        private string _renamePrefix = "";

        private void DrawAddPrefixtab(Object[] objects)
        {
            _renamePrefix = EditorGUILayout.TextField("Prefix", _renamePrefix);

            bool isObjectsValid = IsValidObjectsList(objects);

            EditorGUI.BeginDisabledGroup(!isObjectsValid || string.IsNullOrEmpty(_renamePrefix));
            if (GUILayout.Button("AddPrefix"))
            {
                RenameAssets(objects, name => _renamePrefix + name);
            }
            EditorGUI.EndDisabledGroup();

            if(!isObjectsValid)
            {
                EditorGUILayout.HelpBox("No assets selected", MessageType.Warning);
            }
            if(string.IsNullOrEmpty(_renamePrefix))
            {
                EditorGUILayout.HelpBox("Prefix is empty", MessageType.Warning);
            }
        }

        private string _replaceFrom = "";
        private string _replaceTo = "";

        private void DrawReplaceTab(Object[] objects)
        {
            _replaceFrom = EditorGUILayout.TextField("From", _replaceFrom);
            _replaceTo = EditorGUILayout.TextField("To", _replaceTo);

            bool isObjectsValid = IsValidObjectsList(objects);

            EditorGUI.BeginDisabledGroup(!isObjectsValid || string.IsNullOrEmpty(_replaceFrom) || string.IsNullOrEmpty(_replaceTo));
            if(GUILayout.Button("Replace"))
            {
                RenameAssets(objects, name => name.Replace(_replaceFrom, _replaceTo));
            }
            EditorGUI.EndDisabledGroup();

            if (!isObjectsValid)
            {
                EditorGUILayout.HelpBox("No assets selected", MessageType.Warning);
            }
            if(string.IsNullOrEmpty(_replaceFrom) || string.IsNullOrEmpty(_replaceTo))
            {
                EditorGUILayout.HelpBox("From or To is empty", MessageType.Warning);
            }

            if(isObjectsValid && !string.IsNullOrEmpty(_replaceFrom) && !string.IsNullOrEmpty(_replaceTo))
            {
                Object curObj = objects[0];
                string path = AssetDatabase.GetAssetPath(curObj);
                string name = GetNameFromPath(path);
                string replaced = name.Replace(_replaceFrom, _replaceTo);
                EditorGUILayout.LabelField($"Preview : {replaced}");                
            }
        }

        private string _renameNewName = "";
        private int _renameStartIndex = 0;

        private void DrawRenameTab(Object[] objects)
        {
            _renameNewName = EditorGUILayout.TextField("New Name", _renameNewName);
            _renameStartIndex = EditorGUILayout.IntField("Start Index", _renameStartIndex);

            bool isObjectsValid = IsValidObjectsList(objects);

            EditorGUI.BeginDisabledGroup(!isObjectsValid || string.IsNullOrEmpty(_renameNewName));
            if (GUILayout.Button("Rename"))
            {
                RenameAssets(objects, name => $"{_renameNewName}{_renameStartIndex++}", new NaturalComparer());
            }
            EditorGUI.EndDisabledGroup();

            if (!isObjectsValid)
            {
                EditorGUILayout.HelpBox("No assets selected", MessageType.Warning);
            }
            if (string.IsNullOrEmpty(_renameNewName))
            {
                EditorGUILayout.HelpBox("New Name is empty", MessageType.Warning);
            }
        }

        private void DrawShowTab(Object[] objects)
        {
            var paths = objects.Select(obj => AssetDatabase.GetAssetPath(obj)).Select(path => GetNameFromPath(path)).ToArray();
            paths = paths.OrderBy(x => x, new NaturalComparer()).ToArray();
            foreach(string path in paths)
            {
                EditorGUILayout.LabelField(path);
            }
        }

        /// <summary>
        /// 에셋 이름을 변경. nameProcessor를 통해서 이름 변경 방법을 결정
        /// </summary>
        /// <param name="objects">에셋 이름 배열</param>
        /// <param name="nameProcessor">에셋 이름 함수</param>
        /// <param name="comparer">정렬 방법, null일 경우 정렬하지 않음</param>
        private void RenameAssets(Object[] objects, System.Func<string, string> nameProcessor, IComparer<string> comparer = null)
        {
            var pahtQuery = objects.Select(obj => AssetDatabase.GetAssetPath(obj));
            if(comparer != null)
            {
                pahtQuery = pahtQuery.OrderBy(x => x, comparer);
            }
            var paths = pahtQuery.ToArray();

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

        private bool IsValidObjectsList(Object[] objects)
        {
            return objects != null && objects.Length > 0;
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

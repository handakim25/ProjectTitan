using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

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

            switch((TabIndex)_tabIndex)
            {
                case TabIndex.AddPrefix:
                    DrawAddPrefixtab();
                    break;
                case TabIndex.Replace:
                    DrawReplaceTab();
                    break;
                case TabIndex.Rename:
                    DrawRenameTab();
                    break;
                case TabIndex.Show:
                    DrawShowTab();
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

        private string _renameNewName = "";
        private int _renameStartIndex = 0;

        private void DrawRenameTab()
        {
            _renameNewName = EditorGUILayout.TextField("New Name", _renameNewName);
            _renameStartIndex = EditorGUILayout.IntField("Start Index", _renameStartIndex);

            var objects = GetSelectedAssets();

            EditorGUI.BeginDisabledGroup(objects == null || objects.Length == 0 || string.IsNullOrEmpty(_renameNewName));
            if (GUILayout.Button("Rename"))
            {
                Rename(Selection.objects, _renameNewName, _renameStartIndex);
            }
            EditorGUI.EndDisabledGroup();

            if (objects == null || objects.Length == 0)
            {
                EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            }
            if (string.IsNullOrEmpty(_renameNewName))
            {
                EditorGUILayout.HelpBox("New Name is empty", MessageType.Warning);
            }
        }

        private void Rename(Object[] objects, string renameNewName, int renameStartIndex)
        {
            if (string.IsNullOrEmpty(renameNewName))
            {
                Debug.LogError("New Name is empty");
                return;
            }
            if (objects == null || objects.Length == 0)
            {
                Debug.LogError("No object selected");
                return;
            }

            RenameAssets(objects, name => $"{renameNewName}{renameStartIndex++}", new NaturalComparer());
        }

        private void DrawShowTab()
        {
            var objects = GetSelectedAssets();
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

        // Natural Sort
        // 1. https://blog.codinghorror.com/sorting-for-humans-natural-sort-order/
        // 2. https://stackoverflow.com/questions/1022203/sorting-strings-containing-numbers-in-a-user-friendly-way
        // 일반적으로 구현할려면 굉장히 복잡해서 그나마 정규식 관련 함수를 찾음
        // StrCmpLogicalW 함수는 Windows에 의존적인 함수라서 사용을 하지 않기로 결정
        public class NaturalComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return NaturalCompare(x, y);
            }

            public int NaturalCompare(string x, string y)
            {
                if(x == null && y == null)
                {
                    return 0;
                }
                if(x == null)
                {
                    return -1;
                }
                if(y == null)
                {
                    return 1;
                }

                // 문자열을 숫자와 문자열로 나눈다.
                // \D는 숫자가 아닌 문자, \d는 숫자
                // (?<=\D)(?=\d) : 숫자 앞에 문자가 있는 경우
                // (?<=\d)(?=\D) : 문자 앞에 숫자가 있는 경우
                var regex = new Regex(@"(?<=\D)(?=\d)|(?<=\d)(?=\D)");
                var tokenX = regex.Split(x);
                var tokenY = regex.Split(y);

                for(int i = 0; i < Mathf.Min(tokenX.Length, tokenY.Length); i++)
                {
                    // 둘다 숫자일 경우
                    if (long.TryParse(tokenX[i], out long resultX) && long.TryParse(tokenY[i], out long resultY))
                    {
                        if(resultX != resultY)
                        {
                            return resultX.CompareTo(resultY);
                        }
                    }
                    // 일반 비교, 만약에 동일할 경우 다음 토큰으로 넘어간다.
                    else
                    {
                        int stringCompare = string.Compare(tokenX[i], tokenY[i], System.StringComparison.OrdinalIgnoreCase);
                        if(stringCompare != 0)
                        {
                            return stringCompare;
                        }
                    }
                }

                // 둘 중 하나가 끝났다면
                return tokenX.Length.CompareTo(tokenY.Length);
            }
        }
    }
}

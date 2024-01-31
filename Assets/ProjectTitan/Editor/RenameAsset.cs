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

        private string _prefix = "";

        [MenuItem("Tools/Rename Asset Util")]
        public static void ShowWindow()
        {
            GetWindow<RenameAsset>(true, "Rename Asset");
        }

        private void OnGUI()
        {
            GUILayout.Label("Warning: Undo is not supported yet", WarningStyle);

            _prefix = EditorGUILayout.TextField("Prefix", _prefix);

            var objects = GetSelectedAssets();

            EditorGUI.BeginDisabledGroup(objects == null || objects.Length == 0 || string.IsNullOrEmpty(_prefix));
            if (GUILayout.Button("Rename"))
            {
                Rename(Selection.objects, _prefix);
            }
            EditorGUI.EndDisabledGroup();

            if(objects == null || objects.Length == 0 )
            {
                EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            }
            if(string.IsNullOrEmpty(_prefix))
            {
                EditorGUILayout.HelpBox("Prefix is empty", MessageType.Warning);
            }
        }

        private Object[] GetSelectedAssets()
        {
            return Selection.GetFiltered<Object>(SelectionMode.Assets);
        }

        private void Rename(Object[] objects, string prefix)
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

            var paths = objects.Select(obj => AssetDatabase.GetAssetPath(obj)).ToArray();
            Object[] newObjects = new Object[objects.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                string name = path[(path.LastIndexOf('/') + 1)..];
                name = name[..name.LastIndexOf('.')];
                string err = AssetDatabase.RenameAsset(path, prefix + name);
                if (!string.IsNullOrEmpty(err))
                {
                    Debug.LogError(err);
                }
            }
        }
    }
}

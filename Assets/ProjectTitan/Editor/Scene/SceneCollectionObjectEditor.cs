using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Titan.Core.Scene;

namespace Titan.Scene
{
    [CustomEditor(typeof(SceneCollectionObject))]
    public class SceneCollectionObjectEditor : Editor
    {
        /// <summary>
        /// 생성할 Scene List Asset의 경로. 해당 경로에 SceneList 폴더에 SceneList Asset을 생성한다.
        /// 만약, 해당 경로에 SceneList 폴더가 없을 경우 생성한다.
        /// </summary>
        private static readonly string s_projectPath = "Assets/ProjectTitan/Data";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("Create SceneList Asset"))
            {
                SceneCollectionObject sceneCollection = (SceneCollectionObject)target;
                
                SceneList newScene = GetSceneListAsset(target.name);
                if(newScene == null)
                {
                    Debug.LogError("Failed To Create SceneList Asset");
                    return;
                }

                newScene.scenes = sceneCollection.scenes.Select(scene => scene.name)
                    .Distinct()
                    .ToArray();
                Debug.Log($"Create SceneList Asset : {newScene.name}");

                
                EditorUtility.SetDirty(newScene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Asset들이 생성될 경로를 반환한다. 해당 경로에 폴더가 없을 경우 생성한다.
        /// 폴더의 기본 이름은 SceneList Class의 이름이다.
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns>제대로 존재할 경우 경로를 반환, 없을 경우 ""를 반환한다.</returns>
        static private string GetPath(string assetName)
        {
            var directoryName = typeof(SceneList).Name;
            string directoryPath = System.IO.Path.Combine(s_projectPath, directoryName);

            if (!AssetDatabase.IsValidFolder(directoryPath))
            {
                if(!CreateTargetDirectory(directoryPath))
                {
                    return "";
                }
            }

            return System.IO.Path.Combine(directoryPath, assetName + ".asset");
        }

        /// <summary>
        /// Directory를 생성한다. 중간에 폴더가 없을 경우 생성한다.
        /// </summary>
        /// <param name="directoryPath">생성할 Directory 경로, Assets로 시작한다.</param>
        /// <returns>파일 생성이 실패했을 경우 flase를 반환</returns>
        private static bool CreateTargetDirectory(string directoryPath)
        {
            directoryPath = directoryPath.Replace("\\", "/");
            string[] directories = directoryPath.Split('/');

            // Start from Assets/
            string curPath = directories[0];
            List<string> newDirectoryList = new();
            for (int i = 1; i < directories.Length; ++i)
            {
                string newFolderPath = System.IO.Path.Combine(curPath, directories[i]);
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    newDirectoryList.Add(directories[i]);
                    string guid = AssetDatabase.CreateFolder(curPath, directories[i]);
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError($"Failed to create folder");
                        return false;
                    }
                }
                curPath = newFolderPath;
            }

            if(newDirectoryList.Count > 0)
            {
                var errMsg = string.Join("\n", newDirectoryList.Select(dir => $"Create Directory : {dir}"));
                Debug.Log($"Craete {newDirectoryList.Count} Directories\n{errMsg}");
            }

            return true;
        }

        /// <summary>
        /// SceneList Asset이 이미 있을 경우 로드하고 없을 경우 새로운 Asset을 생성한다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>만약, Asset을 생성하거나 로드하지 못했을 경우 null을 반환한다.</returns>
        static private SceneList GetSceneListAsset(string sceneName)
        {
            string assetPath = GetPath(sceneName + "List");
            if(assetPath == "")
            {
                return null;
            }

            SceneList sceneList;
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SceneList));
            if (asset != null)
            {
                sceneList = asset as SceneList;
            }
            else
            {
                sceneList = SceneList.CreateInstance<SceneList>();
                AssetDatabase.CreateAsset(sceneList, assetPath);
            }
            return sceneList;
        }
    }
}

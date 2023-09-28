using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Titan.Core.Scene;

namespace Titan.Editor.Scene
{
    [CustomEditor(typeof(SceneCollectionObject))]
    public class SceneCollectionObjectEditor : UnityEditor.Editor
    {
        private static readonly string s_projectPath = "Assets/ProjectTitan/Scripts/ScriptableObjects/";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("Create SceneList Asset"))
            {
                SceneCollectionObject sceneCollection = (SceneCollectionObject)target;
                
                SceneList newScene = GetSceneListAsset(target.name);
                newScene.scenes = sceneCollection.scenes.Select(scene => scene.name).ToArray();
                Debug.Log($"Create SceneList Asset : {newScene.name}");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static private string GetPath(string assetName)
        {
            var directoryName = typeof(SceneList).Name;
            string directoryPath = System.IO.Path.Combine(s_projectPath, directoryName);
            if(!AssetDatabase.IsValidFolder(directoryPath))
            {
                Debug.Log($"Create Directory : {directoryPath}");
                if(AssetDatabase.CreateFolder(s_projectPath, directoryName) == "")
                {
                    Debug.Log($"Failed to create folder");
                }
            }

            return System.IO.Path.Combine(directoryPath, assetName + ".asset");
        }

        static private SceneList GetSceneListAsset(string sceneName)
        {
            string assetPath = GetPath(sceneName + "List");
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

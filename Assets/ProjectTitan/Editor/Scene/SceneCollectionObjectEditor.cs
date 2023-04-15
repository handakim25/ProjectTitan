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
        static private string s_projectPath = "Assets/ProjectTitan/Scripts/ScriptableObjects";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("Create SceneList Asset"))
            {
                SceneCollectionObject sceneCollection = (SceneCollectionObject)target;
                
                SceneList newScene = SceneList.CreateInstance<SceneList>();
                newScene.scenes = sceneCollection.scenes.Select(scene => scene.name).ToArray();

                var path = GetPath(target.name + "List");
                AssetDatabase.CreateAsset(newScene, path);
                Debug.Log($"Create Asset : {path}");
                AssetDatabase.Refresh();
            }
        }

        static private string GetPath(string assetName)
        {
            var directoryName = typeof(SceneList).Name;
            string direcotryPath = System.IO.Path.Join(s_projectPath, directoryName);
            if(!AssetDatabase.IsValidFolder(direcotryPath))
            {
                Debug.Log($"Create Directory : {direcotryPath}");
                if(AssetDatabase.CreateFolder(s_projectPath, directoryName) == "")
                {
                    Debug.Log($"Failed to create folder");
                }
            }

            return System.IO.Path.Join(direcotryPath, assetName + ".asset");
        }
    }
}

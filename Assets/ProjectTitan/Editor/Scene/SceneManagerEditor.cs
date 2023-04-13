using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

using Titan.Core.Scene;

namespace Titan.Editor.Scene
{
    using UnityEditor;

    public class SceneManagerEditor : Editor
    {
        [MenuItem("ProjectTitan/Scene/Play")]
        private static void OpenPlayScene()
        {
            LoadScene("GamePlayScenes");
        }

        [MenuItem("ProjectTitan/Scene/Entry")]
        private static void OpenEntryScene()
        {
            LoadScene("EntryScenes");
        }

        private static void LoadScene(string sceneName)
        {
            string[] guid = AssetDatabase.FindAssets(sceneName);
            if(guid.Length > 1)
            {
                Debug.LogWarning($"{sceneName} is duplicated.");
                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guid[0]);
            SceneCollectionObject asset = AssetDatabase.LoadAssetAtPath<SceneCollectionObject>(assetPath);
            if(asset == null)
            {
                Debug.LogWarning($"Cannot find Scene Asset");
                return;
            }
            
            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                asset.Open();
            }
        }
    }
}

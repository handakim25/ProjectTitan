using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core.Scene;

namespace Titan.Editor.Scene
{
    using UnityEditor;

    public class SceneManagerEditor : Editor
    {
        [MenuItem("ProjectTitan/Scene/Play")]
        private static void OpenPlayScene()
        {
            List<SceneCollectionObject> scenes = new List<SceneCollectionObject>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SceneCollectionObject)));
            for(int i = 0; i < guids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                SceneCollectionObject asset = AssetDatabase.LoadAssetAtPath<SceneCollectionObject>(assetPath);
                if(asset != null)
                {
                    scenes.Add(asset);
                }
            }

            foreach(SceneCollectionObject scene in scenes)
            {
                if(scene.name == "GamePlay")
                {
                    scene.Open();
                    return;
                }
            }

            Debug.LogError($"Unalbe to find Play Scene");
        }
    }
}

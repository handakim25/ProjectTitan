using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Titan.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneCollectionObject", menuName = "")]
    public class SceneCollectionObject : ScriptableObject
    {
        public SceneAsset[] scenes;

        public void Open()
        {
            for(int i = 0; i < scenes.Length; ++i)
            {
                SceneAsset scene = scenes[i];
                OpenSceneMode mode = (i == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive;
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), mode);
            }
        }
    }
}

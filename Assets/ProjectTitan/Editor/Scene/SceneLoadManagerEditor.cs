using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
    using UnityEditor;

using Titan.Core.Scene;

namespace Titan.Scene
{
    /// <summary>
    /// Scene Load하는 Editor. 툴바에 추가한다.
    /// </summary>
    public class SceneLoadManagerEditor : Editor
    {
        // @Note
        // 현재까지 파악한 바로는 MenuItem을 이용할 때 동적으로 생성하는 것은 제한적이다.
        // 일단은 동적으로 생성하지 않고 정적으로 이용하도록 한다.

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

        [MenuItem("ProjectTitan/Scene/Title")]
        private static void OpenTitleScene()
        {
            LoadScene("TitleScenes");
        }

        private static void LoadScene(string sceneName)
        {
            string[] guid = AssetDatabase.FindAssets($"t:{typeof(SceneCollectionObject).Name} {sceneName}");
            if(guid.Length == 0)
            {
                Debug.LogWarning($"Cannot find {sceneName}.");
                return;
            }
            else if(guid.Length > 1)
            {
                Debug.LogWarning($"{sceneName} is duplicated. Length:{guid.Length}");
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

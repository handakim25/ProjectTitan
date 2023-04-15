using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnitySceneManger = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Titan.Core.Scene
{
    sealed public class SceneManager : MonoSingleton<SceneManager>
    {        
        [SerializeField] private SceneList defaultScene;

        public void LoadScenes(SceneList list)
        {
            var curScenes = GetAllOpenScenes();
            string[] curSceneNames = curScenes.Select(scene => scene.name).ToArray();

            string[] targetSceneNames = list.scenes;

            var sceneToClose = curSceneNames.Except(targetSceneNames).ToArray();
            var sceneToOpen = targetSceneNames.Except(curSceneNames).ToArray();

            Debug.Log($"Scene To Close");
            foreach(var sceneName in sceneToClose)
            {
                Debug.Log($"name:{sceneName}");
            }
            Debug.Log($"Scene To Open");
            foreach(var sceneName in sceneToOpen)
            {
                Debug.Log($"name:{sceneName}");
            }

            for(int i = 0; i < sceneToClose.Length; ++i)
            {
                UnitySceneManger.UnloadSceneAsync(sceneToClose[i]);
            }

            for(int i = 0; i < sceneToOpen.Length; ++i)
            {
                UnitySceneManger.LoadScene(sceneToOpen[i],LoadSceneMode.Additive);
            }

            if(UnitySceneManger.sceneCount > 0)
            {
                UnityScene activeScene = UnitySceneManger.GetSceneByName(sceneToOpen[0]);
                // UnitySceneManger.SetActiveScene(activeScene);
            }
        }

        public void LoadDefaultScene()
        {
            if(defaultScene == null)
            {
                Debug.LogError($"Missing Default Scene");
            }
            LoadScenes(defaultScene);
        }

        private UnityScene[] GetAllOpenScenes()
        {
            int sceneCount = UnitySceneManger.sceneCount;

            UnityScene[] loadedScenes = new UnityScene[sceneCount];
            for (int i = 0; i < sceneCount; ++i)
            {
                loadedScenes[i] = UnitySceneManger.GetSceneAt(i);
            }
            return loadedScenes;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            if(UnitySceneManger.GetActiveScene().name == "sc_Core")
                LoadDefaultScene();
        }
    }
}

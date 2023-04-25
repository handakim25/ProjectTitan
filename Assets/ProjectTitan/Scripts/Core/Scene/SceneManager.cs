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
        public Camera CoreCamera;
        public GameObject LoadingScreen;
        UnityScene activeScene;
        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        public void LoadScenes(SceneList list)
        {
            LoadingScreen.gameObject.SetActive(true);

            var curScenes = GetAllOpenScenes();
            string[] curSceneNames = curScenes.Select(scene => scene.name).ToArray();

            string[] targetSceneNames = list.scenes;

            var sceneToClose = curSceneNames.Except(targetSceneNames).ToArray();
            var sceneToOpen = targetSceneNames.Except(curSceneNames).ToArray();

            Debug.Log($"--Scene To Close--");
            foreach(var sceneName in sceneToClose)
            {
                Debug.Log($"name:{sceneName}");
            }
            Debug.Log($"--Scene To Open--");
            foreach(var sceneName in sceneToOpen)
            {
                Debug.Log($"name:{sceneName}");
            }

            for(int i = 0; i < sceneToClose.Length; ++i)
            {
                scenesLoading.Add(UnitySceneManger.UnloadSceneAsync(sceneToClose[i]));
            }

            for(int i = 0; i < sceneToOpen.Length; ++i)
            {
                scenesLoading.Add(UnitySceneManger.LoadSceneAsync(sceneToOpen[i],LoadSceneMode.Additive));
            }

            if(UnitySceneManger.sceneCount > 0)
            {
                activeScene = UnitySceneManger.GetSceneByName(sceneToOpen[0]);
            }

            StartCoroutine(GetSceneLoadProgress());
        }

        private IEnumerator GetSceneLoadProgress()
        {
            CoreCamera.gameObject.SetActive(true);
            for(int i = 0; i < scenesLoading.Count; i++)
            {
                while(!scenesLoading[i].isDone)
                {
                    yield return null;
                }
            }

            CoreCamera.gameObject.SetActive(false);
            LoadingScreen.gameObject.SetActive(false);
            UnitySceneManger.SetActiveScene(activeScene);
            GameManager.Instance.SetCameraStack();
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
            UnitySceneManger.sceneLoaded += (scene, mode) => {Debug.Log($"Delegate : name:{scene.name}, mode : {mode}");};

            if(UnitySceneManger.GetActiveScene().name == "sc_Core")
                LoadDefaultScene();
        }
    }
}

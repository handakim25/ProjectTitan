using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnitySceneManger = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Titan.Core.Scene
{
    // Scene이 로드되고 나서 처리해야 할 event를 정의할 필요가 있다.
    // 1. OnSceneLoaded : Load가 완료된 시점. 각 Manager가 필요한 작업을 수행할 수 있다.(e.g. 몬스터 생성이나 Save 데이터 로드 등). 아직 Load의 과정
    // 2. OnStageStart : 모든 Load가 이루어지고 Load가 종료되는 시점

    sealed public class SceneLoadManager : MonoSingleton<SceneLoadManager>
    {        
        [SerializeField] private SceneList defaultScene;
        // LoadScene Camera
        [SerializeField] private Camera CoreCamera;
        [SerializeField] private GameObject LoadingScreen;
        UnityScene activeScene;
        public string ActiveSceneName => activeScene.name;
        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        /// <summary>
        /// Scene Load 완료. Load가 완료되기 전에 호출된다. 이 곳에서 게임 시작 전에 필요한 동작을 한다.
        /// i.e 몬스터 생성, Save 데이터 로드 등
        /// </summary>
        public event System.Action SceneLoaded;

        /// <summary>
        /// 모든 로드가 완료되고 준비가 됬을 때 호출된다. 게임 시작 부분을 처리한다.
        /// </summary>
        public event System.Action SceneStart;

        public void LoadScenes(SceneList list)
        {
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
                activeScene = UnitySceneManger.GetSceneByName(targetSceneNames[0]);
            }

            StartCoroutine(HandleSceneLoadAsync());
        }

        private IEnumerator HandleSceneLoadAsync()
        {
            ShowLoadScene();

            // @To-Do
            // Scene Load 말고도 추가적으로 다른 로딩이 필요할 수 있다.
            // 해당 부분을 처리할 것
            // for(int i = 0; i < scenesLoading.Count; i++)
            // {
            //     while(!scenesLoading[i].isDone)
            //     {
            //         yield return null;
            //     }
            // }
            yield return new WaitUntil(() => scenesLoading.All(scene => scene.isDone));
            UnitySceneManger.SetActiveScene(activeScene);

            HideLoadScene();

            SceneLoaded?.Invoke();
            SceneStart?.Invoke();
        }

        private void ShowLoadScene()
        {
            LoadingScreen.SetActive(true);
            CoreCamera.gameObject.SetActive(true);
        }

        private void HideLoadScene()
        {
            LoadingScreen.SetActive(false);
            CoreCamera.gameObject.SetActive(false);
            
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
            UnitySceneManger.sceneLoaded += OnSceneLoaded;
            HideLoadScene();

            if(UnitySceneManger.GetActiveScene().name == "sc_Core")
            {
                LoadDefaultScene();
                return;
            }

            // 다른 Scene과 같이 로드되었다. 이미 다른 Scene들이 Load된 시점이니 호출해주면 된다.
            // 하지만 호출 순서 문제가 있으므로 프레임을 기다릴 필요가 있다.
            activeScene = UnitySceneManger.GetActiveScene();
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            SceneLoaded?.Invoke(); // 원래라면 이 부분도 비동기 처리. 하지만 지금은 동기로 구현한다.
            SceneStart?.Invoke();
        }

        private void OnSceneLoaded(UnityScene scene, LoadSceneMode mode)
        {
            DebugEditor.Log($"Scene Name : {scene.name}, Load Mode : {mode}");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Tooltip("Core Scene으로 진입했을 때 다른 Scene이 Load되어 있지 않다면 Default Scene을 Load한다.")]
        [FormerlySerializedAs("defaultScene")]
        [SerializeField] private SceneList _defaultScene;
        [Tooltip("로딩 Scene을 Rendering할 Camera")]
        [FormerlySerializedAs("CoreCamera")]
        [SerializeField] private Camera _coreCamera;
        [SerializeField] private LoadUIController _loadUIController;

        UnityScene activeScene;
        public string ActiveSceneName => activeScene.name;
        public bool IsLoading
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Scene Load 완료. Load가 완료되기 전에 호출된다. 이 곳에서 게임 시작 전에 필요한 동작을 한다.
        /// i.e 몬스터 생성, Save 데이터 로드 등
        /// </summary>
        public event System.Action SceneLoaded;

        /// <summary>
        /// 모든 로드가 완료되고 준비가 됬을 때 호출된다. 게임 시작 부분을 처리한다.
        /// </summary>
        public event System.Action SceneStart;

        List<AsyncOperation> scenesLoading = new();
        /// <summary>
        /// List에 있는 Scene들을 Load한다. 만약 기존에 Scene이 존재한다면 유지된다. Scene List에 없는 Scene들은 Unload된다.
        /// </summary>
        /// <param name="list">로드할 Scene List</param>
        public void LoadScenes(SceneList list)
        {
            if(IsLoading)
            {
                Debug.LogError($"Scene Loading 중에 LoadScenes를 호출하였습니다.");
                return;
            }
            if(list == null)
            {
                return;
            }
            IsLoading = true;

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
        
        float totalProgress;
        /// <summary>
        /// Scene이 로드될 때까지 대기하는 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator HandleSceneLoadAsync()
        {
            ShowLoadUI();

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
            // yield return new WaitUntil(() => scenesLoading.All(scene => scene.isDone));
            for(int i = 0; i < scenesLoading.Count; ++i)
            {
                while(!scenesLoading[i].isDone)
                {
                    totalProgress = 0f;
                    foreach(var operation in scenesLoading)
                    {
                        totalProgress += operation.progress;
                    }
                    totalProgress = (totalProgress / scenesLoading.Count) * 100f;
                    _loadUIController.SetProgress(totalProgress);
                    yield return null;
                }
            }
            UnitySceneManger.SetActiveScene(activeScene);

            HideLoadUI();

            // @Refactor
            // 여기서 비동기로 처리해야 되는 부분이 있을 수 있다. 이 부분을 어떻게 처리할 것인지 파악할 것
            SceneLoaded?.Invoke();
            SceneStart?.Invoke();

            IsLoading = false;
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

        public void LoadDefaultScene()
        {
            if(_defaultScene == null)
            {
                Debug.LogError($"Missing Default Scene");
                return;
            }
            LoadScenes(_defaultScene);
        }

        private void ShowLoadUI()
        {
            _loadUIController.gameObject.SetActive(true);
            _coreCamera.gameObject.SetActive(true);
        }

        private void HideLoadUI()
        {
            _loadUIController.gameObject.SetActive(false);
            _coreCamera.gameObject.SetActive(false);
        }

        private void Start()
        {
            UnitySceneManger.sceneLoaded += OnSceneLoaded;
            HideLoadUI();

            if(UnitySceneManger.GetActiveScene().name == "sc_Core")
            {
                LoadDefaultScene();
                return;
            }

            // 다른 Scene과 같이 로드되었다. 이미 다른 Scene들이 Load된 시점이니 호출해주면 된다.
            // 하지만 호출 순서 문제가 있으므로 프레임을 기다릴 필요가 있다.
            activeScene = UnitySceneManger.GetActiveScene();
            StartCoroutine(WaitUntilFrameEnd());
        }

        IEnumerator WaitUntilFrameEnd()
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

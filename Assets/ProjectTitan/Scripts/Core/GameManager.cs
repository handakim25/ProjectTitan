using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using Titan.Core.Scene;
using Titan.Stage;
using Titan.Audio;

namespace Titan.Core
{
    /// <summary>
    /// Game 진행을 위함
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        private GameStatus _status;
        public GameStatus Status => _status;

        [SerializeField] private List<StageDataObject> StageList = new List<StageDataObject>();
        private Dictionary<string, StageDataObject> StageDataDict;
        private StageDataObject _curStage;

        private const string StartPoint = "StartPoint";

        private GameObject Player
        {
            get
            {
                if(_player == null)
                {
                    _player = GameObject.FindGameObjectWithTag("Player");
                }
                return _player;
            }
        }
        private GameObject _player;

        private void Awake()
        {
            _status = GameStatus.None;

            StageDataDict = StageList.ToDictionary(stage => stage.SceneName, stage => stage);
        }

        // If referece between Scenes, cannot be done in start
        private void Start()
        {
            SetCameraStack();

            SceneLoadManager.Instance.SceneLoaded += OnSceneLoaded;
            SceneLoadManager.Instance.SceneStart += OnSceneStart;
        }

        /// <summary>
        /// 현재 열린 씬에서 다른 카메라를 수집해서 설정한다.
        /// - UI Camera
        /// </summary>
        public void SetCameraStack()
        {
            Camera mainCamera = Camera.main;
            if(mainCamera == null)
                return;

            var cameraData = mainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Clear();

            var cameras = Camera.allCameras;
            foreach(Camera camera in cameras)
            {
                if(camera == mainCamera)
                    continue;
                var camData = camera.GetUniversalAdditionalCameraData();
                if(camData.renderType == CameraRenderType.Overlay)
                {
                    cameraData.cameraStack.Add(camera);
                }
            }
        }

        #region Callback
        
        // Stage 기능을 일단 이곳에서 사용한다.
        // 양이 많아지면 분리한다.

        private void OnSceneLoaded()
        {
            string sceneName = SceneLoadManager.Instance.ActiveSceneName;
            Debug.Log($"Active Scene : {sceneName}");
            if(!string.IsNullOrEmpty(sceneName) && StageDataDict.ContainsKey(sceneName))
            {
                StageDataObject stage = StageDataDict[sceneName];
                ReadyScene(stage);
            }
            else
            {
                // Stage가 아닐 경우, Title 등의 상황
                _curStage = null;
            }
        }

        private void ReadyScene(StageDataObject stage)
        {
            SetCameraStack();
            _curStage = stage;

            if(Player == null)
            {
                Debug.LogError("Player is not found");
                return;
            }

            var startPoint = GameObject.FindGameObjectWithTag(StartPoint);
            if(startPoint)
            {
                Player.transform.position = startPoint.transform.position;
                Player.transform.SetPositionAndRotation(startPoint.transform.position, startPoint.transform.rotation);
            }
            else
            {
                Debug.LogError("StartPoint is not found");
                Player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        private void OnSceneStart()
        {
            // Stage가 아닐 경우
            if(_curStage == null)
            {
                return;
            }
            Debug.Log($"OnSceneLoaded : {_curStage.SceneName}");

            SoundManager.Instance.PlayBGM((int)_curStage.BGM);
            
        }
        
        #endregion Callback
    }
}

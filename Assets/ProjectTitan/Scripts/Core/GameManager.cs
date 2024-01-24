using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using Titan.Core.Scene;
using Titan.Character;
using Titan.Stage;
using Titan.Audio;
using Titan.UI;
using Titan.Character.Player;
using System;

namespace Titan.Core
{
    /// <summary>
    /// Game 진행을 위함
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        private GameStatus _status;
        public GameStatus Status => _status;

        // @To-Do
        // Stage 기능을 따로 분리하는 것을 고려할 것
        [SerializeField] private List<StageDataObject> StageList = new();
        private Dictionary<string, StageDataObject> StageDataDict;
        private StageDataObject _curStage;

        private const string StartPoint = "StartPoint";

        private GameObject Player => _player != null ? _player : _player = GameObject.FindGameObjectWithTag("Player");
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

        /// <summary>
        /// Scene이 로드되면 호출된다. 아직 로딩이 끝나지 않은 단계
        /// </summary>
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

        /// <summary>
        /// GameManager가 모든 준비를 끝냈을 경우, 다른 Scene이 로드된 것을 보장
        /// </summary>
        /// <param name="stage"></param>
        private void ReadyScene(StageDataObject stage)
        {
            SetCameraStack();
            _curStage = stage;

            InitPlayer();
            InitHudUI();
            InitCamera();
            InitQuest(stage._startQuests);
        }

        private static void InitQuest(List<QuestSystem.QuestObject> startQuests)
        {
            foreach (var quest in startQuests ?? Enumerable.Empty<QuestSystem.QuestObject>())
            {
                EventBus.RaiseEvent(new QuestEvent()
                {
                    QuestID = quest.ID,
                    Status = QuestSystem.QuestStatus.Received,
                });
            }
        }

        private void InitPlayer()
        {
            if (Player == null)
            {
                Debug.LogError("Player is not found");
                return;
            }

            // Reset Player Position
            var startPoint = GameObject.FindGameObjectWithTag(StartPoint);
            if (startPoint)
            {
                Player.transform.position = startPoint.transform.position;
                Player.transform.SetPositionAndRotation(startPoint.transform.position, startPoint.transform.rotation);
            }
            else
            {
                Debug.LogError("StartPoint is not found");
                Player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            // Player와 UI 연결
            if(Player.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.InitPlayer();
                var hudController = UIManager.Instance.HudUIController;
                if(hudController != null)
                {
                    hudController.InitPlayerView(playerController.Status);
                    playerController.PlayerDataChanged += hudController.UpdatePlayerData;
                }
                playerController.ForceUpdateStatus();
            }
        }

        private void InitHudUI()
        {
            // Stage 이름을 초기화
            var hudController = UIManager.Instance.HudUIController;
            if(hudController != null)
            {
                hudController.UpdateStageName(_curStage.StageName);
            }
        }

        private void InitCamera()
        {
            Billboard.TargetCamera = Camera.main;
            var playerCam = Player.GetComponentInChildren<PlayerCameraController>();
            if(playerCam != null)
            {
                playerCam.InitCameraPos();
            }
        }

        /// <summary>
        /// 모든 Scene이 로드되었고 준비가 끝났을 때 호출된다.
        /// 즉, 게임의 시작 부분이다.
        /// </summary>
        private void OnSceneStart()
        {
            // Stage가 아닐 경우
            if(_curStage == null)
            {
                return;
            }
            Debug.Log($"Stage Loaded : {_curStage.SceneName}");

            SoundManager.Instance.PlayBGM((int)_curStage.BGM);
            if(Player.TryGetComponent<PlayerInput>(out var input))
            {
                input.InputEnable = true;
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            _status = GameStatus.Pause;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _status = GameStatus.Play;
        }


        #endregion Callback
    }
}

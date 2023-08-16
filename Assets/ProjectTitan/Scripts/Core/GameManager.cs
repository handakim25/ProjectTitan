using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Titan.Core
{
    /// <summary>
    /// Game 진행을 위함
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        private GameStatus _status;
        public GameStatus Status => _status;

        // If referece between Scenes, cannot be done in start
        private void Start()
        {
            SetCameraStack();
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
    }
}

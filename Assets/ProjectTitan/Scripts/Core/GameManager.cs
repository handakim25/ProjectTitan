using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Titan.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private GameStatus _status;
        public GameStatus Status => _status;

        // If referece between Scenes, cannot be done in start
        private void Start()
        {
            SetCameraStack();
        }

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

                cameraData.cameraStack.Add(camera);
            }
        }
    }
}

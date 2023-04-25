using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Titan.Core
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            SetCameraStack();
        }

        public void SetCameraStack()
        {
            Camera mainCamera = Camera.main;
            var cameraData = mainCamera.GetUniversalAdditionalCameraData();

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

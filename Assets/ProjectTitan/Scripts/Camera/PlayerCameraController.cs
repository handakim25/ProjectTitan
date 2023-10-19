using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Titan
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class PlayerCameraController : MonoBehaviour
    {
        private float _initCamX;
        private float _initCamY;

        protected CinemachineFreeLook freeLookCamera;

        private void Awake()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            _initCamX = freeLookCamera.m_XAxis.Value;
            _initCamY = freeLookCamera.m_YAxis.Value;
        }

        public void InitCameraPos()
        {
            freeLookCamera.m_XAxis.Value = _initCamX;
            freeLookCamera.m_YAxis.Value = _initCamY;

            if(freeLookCamera.Follow != null)
            {
                var eulerAngle = freeLookCamera.Follow.transform.eulerAngles;
                freeLookCamera.m_Heading.m_Bias = eulerAngle.y;
            }
        }
    }
}

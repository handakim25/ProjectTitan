using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class StartCamera : MonoBehaviour
    {
        [SerializeField] private GameObject _playerCam;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _lookAt;

        void Start()
        {
            // var plyaerCam = Instantiate(_playerCam, transform.position, transform.rotation);
            var playerCam = Instantiate(_playerCam, transform.position, transform.rotation, _target.transform);
            Debug.Log($"Player Cam : {playerCam.transform.position}");
            if(playerCam.TryGetComponent(out Cinemachine.CinemachineFreeLook cam))
            {
                cam.Follow = _target;
                cam.LookAt = _lookAt;
                // cam.ForceCameraPosition(transform.position, transform.rotation);
                // cam.transform.SetPositionAndRotation(transform.position, transform.rotation);
                cam.UpdateCameraState(Vector3.up, 0f);
                Debug.Log($"Cam Pos : {cam.transform.position}");
            }
            else
            {
                Debug.LogError("Player Camera not found");
            }
        }
    }
}

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
            var plyaerCam = Instantiate(_playerCam, transform.position, transform.rotation);
            Debug.Log($"Player Cam : {plyaerCam.transform.position}");
            if(plyaerCam.TryGetComponent(out Cinemachine.CinemachineFreeLook cam))
            {
                cam.Follow = _target;
                cam.LookAt = _lookAt;
                cam.ForceCameraPosition(transform.position, transform.rotation);
            }
            else
            {
                Debug.LogError("Player Camera not found");
            }
        }
    }
}

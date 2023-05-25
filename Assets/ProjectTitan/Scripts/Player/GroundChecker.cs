using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character
{

    /// <summary>
    /// Ground Check 코드를 따로 분리하기 위함
    /// Player 뿐만이 아니라 다른 곳에서도 사용될 수도 있을 수도 있고
    /// </summary>
    public class GroundChecker : MonoBehaviour
    {
        public enum DetectMethod
        {
            Raycast,
            SpehereCast,
        }

        [SerializeField] LayerMask _groundMask = TagAndLayer.LayerMasking.Ground;
        [SerializeField] private float _checkDistance = 0.1f;
        [SerializeField] private DetectMethod _detectMethod;
        DetectMethod Detect => _detectMethod = DetectMethod.Raycast;

        private CharacterController _controller;
        private Vector3 _colExtents;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if(TryGetComponent<Collider>(out var collider))
            {
                // Capsule Collider
                // raidus : 0.3
                // height : 1.8
                // -> extents : 0.3, 0.9, 0.3 // x, y, z
                // x, z : 반지름
                // y : 높이의 절반
                _colExtents = collider.bounds.extents;
            }
        }

        public bool IsGround()
        {
            switch(_detectMethod)
            {
                case DetectMethod.Raycast:
                    return IsGroundRay();
                case DetectMethod.SpehereCast:
                    return IsGroundSphere();
                default:
                    return IsGroundRay();
            }
        }

        private bool IsGroundRay()
        {
            // reference code : https://forum.unity.com/threads/charactercontroller-and-walking-down-a-stairs.101859/
            float floorDistFromFoot = _controller?.stepOffset ?? 0.0f;
            if(Physics.Raycast(transform.position + Vector3.up * _checkDistance, Vector3.down, floorDistFromFoot + _checkDistance, TagAndLayer.LayerMasking.Ground))
                return true;
            return false;
        }

        private bool IsGroundSphere()
        {
            // _colExtents.x * 2 : 구 하나만큼 위로 이동
            // SphereCast(ray, _colExtents.x, _colExtents.x + 0.2f)
            // 그림을 그려서 보면 이해하기 쉽다.
            // Spherecast이기 때문에 2r 만큼 위로 올려서 sphere를 생성하고
            // r +  0.2만큼 이동하면 밑바닥으로부터 0.2보다 더 간 만큼 접촉을 체크한다.
            Ray ray = new Ray(transform.position + Vector3.up * 2 * _colExtents.x, Vector3.down);
            return Physics.SphereCast(ray, _colExtents.x, _colExtents.x + 0.2f);
        }
    }
}

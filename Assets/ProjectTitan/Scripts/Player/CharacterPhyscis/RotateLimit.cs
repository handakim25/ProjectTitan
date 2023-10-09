using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class RotateLimit : MonoBehaviour
    {
        [SerializeField] private float _maxAngle = 45f;
        [SerializeField] private float _minAngle = -45f;
        [SerializeField] private Vector3 rotAxis = Vector3.up;

        private Quaternion _initLocalRot;

        private void Awake()
        {
            _initLocalRot = transform.localRotation;
        }

        private void FixedUpdate()
        {
            Quaternion curRot = transform.localRotation;

            Quaternion deltaRot = Quaternion.FromToRotation(_initLocalRot * rotAxis, curRot * rotAxis);
            float angle = deltaRot.eulerAngles.y;

            angle = Mathf.Clamp(angle, _minAngle, _maxAngle);

            Quaternion clampRot = Quaternion.AngleAxis(angle, rotAxis);
            transform.localRotation = _initLocalRot * clampRot;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(Vector3.zero, rotAxis);
        }
    }
}

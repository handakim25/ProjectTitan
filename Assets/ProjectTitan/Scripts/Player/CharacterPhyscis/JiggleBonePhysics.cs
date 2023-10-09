using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    // 참조 : https://wergia.tistory.com/72
    // Character Joint 설정 참조 : https://wergia.tistory.com/68
    public class JiggleBonePhysics : MonoBehaviour
    {
        [SerializeField] private float Power = 1f;
        [SerializeField] private float ClampDist = 0.03f;

        private Transform _parentTransform;
        private Vector3 _prevParentPos;
        private Rigidbody _rbody;

        private void Awake()
        {
            _parentTransform = transform.parent;
            _prevParentPos = _parentTransform.position;

            _rbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 delta = _parentTransform.position - _prevParentPos;
            _rbody.AddForce(Vector3.ClampMagnitude(delta, ClampDist) * Power);
            _prevParentPos = _parentTransform.position;
        }
    }
}

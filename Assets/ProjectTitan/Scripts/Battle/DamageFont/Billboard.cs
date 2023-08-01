using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class Billboard : MonoBehaviour
    {
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cam.transform.forward, _cam.transform.up);
        }
    }
}

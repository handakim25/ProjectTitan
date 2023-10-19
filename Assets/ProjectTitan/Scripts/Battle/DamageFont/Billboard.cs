using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class Billboard : MonoBehaviour
    {
        public static Camera TargetCamera;

        private void LateUpdate()
        {
            if(TargetCamera == null)
            {
                return;
            }

            transform.LookAt(transform.position + TargetCamera.transform.forward, TargetCamera.transform.up);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform targetTransform;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            if(targetTransform == null)
            {
                targetTransform = GameObject.FindWithTag("Player").transform;
            }
        }

        private void LateUpdate()
        {
            Vector3 nextPos = targetTransform.position;
            nextPos.y = 100f;
            transform.position = nextPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class LogTransform : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            Debug.Log($"Position : {transform.position}");
        }
    }
}

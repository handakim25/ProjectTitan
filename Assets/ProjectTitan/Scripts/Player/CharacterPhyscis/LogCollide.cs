using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public class LogCollide : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("OnCollisionEnter : " + other.gameObject.name);
        }
    }
}

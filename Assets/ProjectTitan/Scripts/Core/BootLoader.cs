using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Titan.Core
{
    public class BootLoader : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene("sc_Core");
        }
    }
}

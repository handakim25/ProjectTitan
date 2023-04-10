using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Titan
{
    public class BootLoader : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene("sc_Core");
        }
    }
}

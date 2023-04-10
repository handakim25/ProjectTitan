using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnitySceneManger = UnityEngine.SceneManagement.SceneManager;

namespace Titan.Core.Scene
{
    sealed public class SceneManager : MonoSingleton<SceneManager>
    {        
        public void LoadDefaultScene()
        {
            // @refactor
            // Set scene as Scene collections
            UnitySceneManger.LoadScene("level_Test");
            UnitySceneManger.LoadScene("sc_UI", LoadSceneMode.Additive);
            UnitySceneManger.LoadScene("sc_Light", LoadSceneMode.Additive);
            UnitySceneManger.LoadScene("sc_test", LoadSceneMode.Additive);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            LoadDefaultScene();
        }
    }
}

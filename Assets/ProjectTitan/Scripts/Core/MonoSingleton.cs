using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance = null;
        private static bool appIsClosing = false;

        public static T Instance
        {
            get
            {
                if(appIsClosing)
                {
                    return null;
                }
                if(s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                    if(s_instance == null)
                    {
                        Debug.LogWarning($"No instance of {typeof(T)}. Temporally create instance.");
                        GameObject go = new GameObject(typeof(T).ToString());
                        s_instance = go.AddComponent<T>();
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if(s_instance == null)
            {
                s_instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        private void OnApplicationQuit()
        {
            s_instance = null;
            appIsClosing = true;
        }
    }
}

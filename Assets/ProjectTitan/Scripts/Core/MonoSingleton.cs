using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Core
{
    /// <summary>
    /// Singleton 중에서 MonoBehaviour를 상속 받은 Singleton Class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool _dontDestroyOnLoad = false;
        private static T s_instance = null;
        private static bool appIsClosing = false;

        public static T Instance
        {
            get
            {
                // app이 닫힐 때는 반환하지 않는다.
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

        private void Awake()
        {
            Debug.Log($"Awake {typeof(T)}");
            if(s_instance == null)
            {
                s_instance = this as T;
                if(_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            s_instance = null;
            appIsClosing = true;
        }
    }
}

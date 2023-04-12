using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public virtual void OpenUI()
        {
            Debug.Log($"Open UI");
            gameObject.SetActive(true);
        }

        public virtual void CloseUI()
        {
            Debug.Log($"Close UI");
            gameObject.SetActive(false);
        }
    }
}

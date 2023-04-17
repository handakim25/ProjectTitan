using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    // Component?
    // Inheritence?
    public abstract class UIScene : MonoBehaviour
    {
        public virtual void OpenUI(float transitionTime)
        {
            Debug.Log($"Open UI");
            gameObject.SetActive(true);
        }

        public virtual void CloseUI(float transitionTime)
        {
            Debug.Log($"Close UI");
            gameObject.SetActive(false);
        }
    }
}

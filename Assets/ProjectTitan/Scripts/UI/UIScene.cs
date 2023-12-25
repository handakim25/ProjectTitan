using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    // Component?
    // Inheritence?
    public class UIScene : MonoBehaviour
    {
        public bool shouldTimeStop = true;
        public bool shouldBlur = true;

        public virtual void OpenUI()
        {
            Debug.Log($"Open UI");
        }

        public virtual void CloseUI()
        {
            Debug.Log($"Close UI");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    // Component?
    // Inheritence?
    public class UIScene : MonoBehaviour
    {
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

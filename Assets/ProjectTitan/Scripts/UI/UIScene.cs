using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    // Component?
    // Inheritence?
    public class UIScene : MonoBehaviour
    {
        /// <summary>
        /// UI가 열릴 때 Time을 멈추는지 여부
        /// </summary>
        [Header("UI Settings")]
        [Tooltip("UI가 열릴 때 Time을 멈추는지 여부")]
        public bool shouldTimeStop = true;
        /// <summary>
        /// UI가 열릴 때 Blur 처리를 할지 여부
        /// </summary>
        [Tooltip("UI가 열릴 때 Blur 처리를 할지 여부")]
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

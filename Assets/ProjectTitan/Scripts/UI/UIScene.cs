using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Audio;

namespace Titan.UI
{
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

        [Header("Sound")]
        [SerializeField] private SoundClip _openSound;
        [SerializeField] private SoundClip _closeSound;

        public void OpenUI()
        {
            gameObject.SetActive(true);
            UIManager.Instance.OpenUIScene(this);

            HandleUIOpen();
        }

        public void CloseUI()
        {
            HandleUIClose();
        }

        /// <summary>
        /// UI가 열릴 때 호출된다. OpenUI에서 gameObject를 활성화하므로 처리할 필요 없다.
        /// </summary>
        protected virtual void HandleUIOpen() {}
        /// <summary>
        /// UI가 닫힐 때 호출된다. 애니메이션이 진행이 되어야 하므로 gameObject를 비활성화하는 것은 HandleUIClose에서 호출한다.
        /// </summary>
        protected virtual void HandleUIClose() {}
    }
}

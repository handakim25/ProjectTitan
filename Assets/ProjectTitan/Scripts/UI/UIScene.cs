using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Audio;

namespace Titan.UI
{
    /// <summary>
    /// UI 하나의 Scene을 관리한다. UI Scene이 열리고 닫혔을 때 Animation을 처리하고 있다.
    /// Game Object의 활성화를 관리하는 것은 UIScene이 담당하고 있다.
    /// 현재 중첩된 Scene은 처리하지 않는다.
    /// </summary>
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
        [Tooltip("UI가 열릴 때 Sound Clip")]
        [SerializeField] private SoundList _openSound = SoundList.None;
        [Tooltip("UI가 닫힐 때 Sound Clip")]
        [SerializeField] private SoundList _closeSound = SoundList.None;

        /// <summary>
        /// UI를 열게 한다.
        /// </summary>
        public void OpenUI()
        {
            // 이미 열려있는 경우에는 무시한다.
            if(gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(true);
            SoundManager.Instance.PlayUISound((int)_openSound);
            UIManager.Instance.OpenUIScene(this);
        }

        /// <summary>
        /// UI를 닫게 한다.
        /// </summary>
        public void CloseUI()
        {
            // 이미 닫혀있는 경우에는 무시한다.
            if(!gameObject.activeSelf)
            {
                return;
            }

            SoundManager.Instance.PlayUISound((int)_closeSound);
            HandleUIClose();
        }

        /// <summary>
        /// OnEnable에서 활성화 될 때 Animation을 재생
        /// </summary>
        protected virtual void OnEnable() {}
        /// <summary>
        /// UI가 닫힐 때 호출된다. 애니메이션이 진행이 되어야 하므로 gameObject를 비활성화하는 것은 HandleUIClose에서 호출한다.
        /// </summary>
        protected virtual void HandleUIClose() {}
    }
}

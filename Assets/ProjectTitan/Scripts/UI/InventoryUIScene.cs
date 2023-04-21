using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan.UI
{
    public class InventoryUIScene : UIScene
    {
        private CanvasGroup _canvasGroup;
        [SerializeField] private float _transitionTime = 0.5f;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // 활성화 되지 않은 오브젝트의 경우는
            // Awake 호출이 늦게 될 수 있다.
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void OpenUI()
        {
            gameObject.SetActive(true);

            UIManager.Instance.OpenUIScene(this);

            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1.0f, 0.0f).SetDelay(_transitionTime);
        }

        public override void CloseUI()
        {
            gameObject.SetActive(false);

            UIManager.Instance.CloseUISceneHandler();
        }
    }
}

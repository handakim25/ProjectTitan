using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan.UI
{
    /// <summary>
    /// Scene 전체를 관리
    /// </summary>
    public class InventoryUIScene : UIScene
    {
        private CanvasGroup _canvasGroup;

        // 상단에는 상속 받은 UI Scene의 설정이 있다.
        [Space]
        [SerializeField] private float _transitionTime = 0.5f;

        private void Awake()
        {
            // @Memo
            // 비활성화 상태에서 Scene으로 로드되면 Awake는 호출되지 않는다.
            // 만약에 다른 함수를 호출하고 싶다면 먼저 활성화 상태로 만들어서 초기화를 진행해야 한다.
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1.0f, 0.0f)
                        .SetDelay(_transitionTime)
                        .SetUpdate(true);
        }

        protected override void HandleUIClose()
        {
            gameObject.SetActive(false);

            UIManager.Instance.CloseUIScene(this);
        }
    }
}

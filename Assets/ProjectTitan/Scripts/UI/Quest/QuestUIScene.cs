using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan.UI
{
    public class QuestUIScene : UIScene
    {
        private CanvasGroup _canvasGroup;
        [SerializeField] private float _transitionTime = 0.5f;

        private void Awake()
        {
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

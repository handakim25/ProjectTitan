using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan.UI
{
    public class HudUIController : UIScene
    {
        [Header("UI")]
        // 생각보다 이렇게 연결하는 것 불편하네
        // 다음 프로젝트는 조금 더 스케일러블한 구조를 생각해 볼 것

        // 이거 단순하게 사용되는 데 그냥 RectTransfrom을 받아오면 되지 않을까?
        [SerializeField] GameObject _upperBar;
        [SerializeField] GameObject _interactionPannel;
        [SerializeField] GameObject _healthPannel;
        [SerializeField] GameObject _minimapPannel;
        [SerializeField] GameObject _skillPannel;

        [Header("Animation")]
        [SerializeField] float _transitionTime;
        [SerializeField] Ease openEaseType;
        [SerializeField] Ease closeEaseType;

        #region UIScene

        public override void OpenUI()
        {
            _upperBar.SetActive(true);
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(openEaseType);

            // move to right
            _interactionPannel.SetActive(true);
            var interactRect = _interactionPannel.GetComponent<RectTransform>();
            interactRect.DOAnchorPosX(0, _transitionTime)
                .SetEase(openEaseType);

            _healthPannel.SetActive(true);
            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(openEaseType);

            // move from left
            _minimapPannel.SetActive(true);
            var minimapRct = _minimapPannel.GetComponent<RectTransform>();
            minimapRct.DOAnchorPosX(0, _transitionTime)
                .SetEase(openEaseType);

            _skillPannel.SetActive(true);
            var skillRect = _skillPannel.GetComponent<RectTransform>();
            skillRect.DOAnchorPosX(0, _transitionTime)
                .SetEase(openEaseType);
        }

        public override void CloseUI()
        {
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(upperRect.sizeDelta.y, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete( () => _upperBar.SetActive(false));

            // move from right
            _interactionPannel.SetActive(false);
            var interactRect = _interactionPannel.GetComponent<RectTransform>();
            interactRect.DOAnchorPosX(interactRect.sizeDelta.x, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _interactionPannel.SetActive(false));

            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(-healthRect.sizeDelta.y, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _healthPannel.SetActive(false));

            // move left
            var minimapRct = _minimapPannel.GetComponent<RectTransform>();
            minimapRct.DOAnchorPosX(-minimapRct.sizeDelta.x, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _minimapPannel.SetActive(false));

            var skillRect = _skillPannel.GetComponent<RectTransform>();
            skillRect.DOAnchorPosX(skillRect.sizeDelta.x, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _skillPannel.SetActive(false));
        }
        
        #endregion UIScene
    }
}

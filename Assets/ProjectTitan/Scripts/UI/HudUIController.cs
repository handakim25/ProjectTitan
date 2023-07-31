using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan.UI
{
    // Some elements in ui -> Open certain scene
    // Stack이 될지 안 될 지는 Scene이 판단할 일이 아니야
    // 
    public class HudUIController : UIScene
    {
        [SerializeField] GameObject _upperBar;
        [SerializeField] GameObject _healthPannel;
        [SerializeField] float _transitionTime;

        #region UIScene
        
        public override void OpenUI()
        {
            _upperBar.SetActive(true);
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(Ease.OutCubic);

            _healthPannel.SetActive(true);
            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(Ease.OutCubic);
        }

        public override void CloseUI()
        {
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(upperRect.sizeDelta.y, _transitionTime)
                .SetEase(Ease.InCubic)
                .OnComplete( () => _upperBar.SetActive(false));

            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(-healthRect.sizeDelta.y, _transitionTime)
                .SetEase(Ease.InCubic)
                .OnComplete(() => _healthPannel.SetActive(false));
        }
        
        #endregion UIScene
    }
}

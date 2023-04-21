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
        [SerializeField] float _transitionTime;

        #region UIScene
        
        public override void OpenUI()
        {
            _upperBar.SetActive(true);
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(Ease.OutCubic);
        }

        public override void CloseUI()
        {
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(upperRect.sizeDelta.y, _transitionTime)
                .SetEase(Ease.InCubic)
                .OnComplete( () => _upperBar.SetActive(false));
        }
        
        #endregion UIScene
    }
}

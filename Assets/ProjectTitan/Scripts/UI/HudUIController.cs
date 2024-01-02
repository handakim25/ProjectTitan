using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using DG.Tweening;

using Titan.Character.Player;

// @Refacactor
// 추후에 조금 더 Scalable한 구조로 변경 필요

namespace Titan.UI
{
    /// <summary>
    /// Hud를 관리하는 Controller
    /// </summary>
    public class HudUIController : UIScene
    {
        [Header("UI")]
        [Tooltip("상단바")]
        [SerializeField] GameObject _upperBar;
        [Tooltip("상호작용 패널")]
        [SerializeField] GameObject _interactionPannel;
        /// <summary>
        /// 상호작용 패널의 초기 위치, 상호작용 패널이 열릴때 이 위치로 이동
        /// </summary>
        private float _interactionStartAnchorX;

        [Tooltip("체력 패널")]
        [SerializeField] GameObject _healthPannel;
        [Tooltip("미니맵 패널")]
        [SerializeField] GameObject _minimapPannel;
        [Tooltip("스킬 패널")]
        [SerializeField] GameObject _skillPannel;
        /// <summary>
        /// Skill Pannel을 관리하는 Controller
        /// </summary>
        private SkillPannelController _skillPannelController;

        [Header("Animation")]
        [Tooltip("UI가 열릴때 걸리는 시간")]
        [SerializeField] float _transitionTime;
        [Tooltip("UI 열리는 Ease Type")]
        [SerializeField] Ease openEaseType;
        [Tooltip("UI 닫히는 Ease Type")]
        [SerializeField] Ease closeEaseType;

        #region Unity Methods
        
        private void Awake()
        {
            if(_skillPannel != null)
            {
                _skillPannelController = _skillPannel.GetComponent<SkillPannelController>();
            }
        }
        private void Start()
        {
            _interactionStartAnchorX = _interactionPannel.GetComponent<RectTransform>().anchoredPosition.x;
        }
        
        #endregion Unity Methods

        #region UIScene

        public override void OpenUI()
        {
            // move from top
            _upperBar.SetActive(true);
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(openEaseType)
                .SetUpdate(true);

            // move from right
            _interactionPannel.SetActive(true);
            var interactRect = _interactionPannel.GetComponent<RectTransform>();
            var interactCanvasGroup = _interactionPannel.GetComponent<CanvasGroup>();
            var interactSequnce = DOTween.Sequence();
            interactSequnce.Append(interactRect.DOAnchorPosX(_interactionStartAnchorX, _transitionTime).SetEase(openEaseType));
            interactSequnce.Join(interactCanvasGroup.DOFade(1, _transitionTime).SetEase(openEaseType)).SetUpdate(true);
            
            // move from bottom
            _healthPannel.SetActive(true);
            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(0, _transitionTime)
                .SetEase(openEaseType)
                .SetUpdate(true);

            // move from left
            _minimapPannel.SetActive(true);
            var minimapRct = _minimapPannel.GetComponent<RectTransform>();
            minimapRct.DOAnchorPosX(0, _transitionTime)
                .SetEase(openEaseType)
                .SetUpdate(true);

            // move from right
            _skillPannel.SetActive(true);
            var skillRect = _skillPannel.GetComponent<RectTransform>();
            skillRect.DOAnchorPosX(0, _transitionTime)
                .SetEase(openEaseType)
                .SetUpdate(true);
        }

        public override void CloseUI()
        {
            // move to top
            var upperRect = _upperBar.GetComponent<RectTransform>();
            upperRect.DOAnchorPosY(upperRect.sizeDelta.y, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete( () => _upperBar.SetActive(false))
                .SetUpdate(true);

            // move to right
            var interactRect = _interactionPannel.GetComponent<RectTransform>();
            var interactCanvasGroup = _interactionPannel.GetComponent<CanvasGroup>();
            var interactSequnce = DOTween.Sequence();
            interactSequnce.Append(interactRect.DOAnchorPosX(0, _transitionTime).SetEase(closeEaseType));
            interactSequnce.Join(interactCanvasGroup.DOFade(0, _transitionTime).SetEase(closeEaseType));
            interactSequnce.OnComplete(() => _interactionPannel.SetActive(false))
                            .SetUpdate(true);

            // move to bottom
            var healthRect = _healthPannel.GetComponent<RectTransform>();
            healthRect.DOAnchorPosY(-healthRect.sizeDelta.y, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _healthPannel.SetActive(false))
                .SetUpdate(true);

            // move to left
            var minimapRct = _minimapPannel.GetComponent<RectTransform>();
            minimapRct.DOAnchorPosX(-minimapRct.sizeDelta.x, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _minimapPannel.SetActive(false))
                .SetUpdate(true);

            // move to right
            var skillRect = _skillPannel.GetComponent<RectTransform>();
            skillRect.DOAnchorPosX(skillRect.sizeDelta.x, _transitionTime)
                .SetEase(closeEaseType)
                .OnComplete(() => _skillPannel.SetActive(false))
                .SetUpdate(true);
        }

        #endregion UIScene

        /// <summary>
        /// Stage 이름을 변경
        /// </summary>
        /// <param name="stageName">바꿀 Stage 이름</param>
        public void UpdateStageName(string stageName)
        {
            if(_minimapPannel == null)
            {
                Debug.LogError("MinimapPannel is not found");
                return;
            }
            var placeName = _minimapPannel.transform.Find("PlaceHolder/PlaceName");
            if(placeName != null && placeName.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                text.text = stageName; 
            }
            else
            {
                Debug.LogError("Cannot find stage name text");
            }
        }

        public void InitPlayerView(PlayerStatus status)
        {
            if(_skillPannelController != null)
            {
                _skillPannelController.InitSkillData(status);
            }
            else
            {
                Debug.LogError("Skill Pannel Controller is not found");
            }
        }

        /// <summary>
        /// Update Ui by player status
        /// </summary>
        /// <param name="status"></param>
        public void UpdatePlayerData(PlayerStatus status)
        {
            // // Skill Control
            if(_skillPannelController != null)
            {
                _skillPannelController.UpdateSkillData(status);
            }
            else
            {
                Debug.LogError("Skill Pannel Controller is not found");
            }
        }
    }
}

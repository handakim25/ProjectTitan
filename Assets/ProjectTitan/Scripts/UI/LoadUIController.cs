using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

namespace Titan
{
    /// <summary>
    /// Load Scene에서 사용되는 UI를 관리하는 클래스
    /// </summary>
    public class LoadUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _progreeText;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _tipText;

        [Header("Resource")]
        [SerializeField] private Sprite _backgroudnSprite;

        private void Awake()
        {
            Assert.IsNotNull(_backgroundImage);
            Assert.IsNotNull(_progreeText);
            Assert.IsNotNull(_progressSlider);
            Assert.IsNotNull(_tipText);
        }

        public Sprite BackgroundSprite
        {
            get => _backgroudnSprite;
            set
            {
                _backgroudnSprite = value;
                if(_backgroundImage != null)
                {
                    _backgroundImage.sprite = _backgroudnSprite;
                }
            }
        }

        /// <summary>
        /// Progress UI를 설정한다. [0,1]
        /// </summary>
        /// <param name="progress">[0,1]</param>
        private void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            if (_progressSlider != null)
            {
                _progressSlider.value = progress;
            }
            if (_progreeText != null)
            {
                _progreeText.text = $"{progress * 100:F2}%";
            }
        }

        public string TooltipText
        {
            get => _tipText != null ? _tipText.text : "";
            set
            {
                if (_tipText != null)
                {
                    _tipText.text = value;
                }
            }
        }
    }
}

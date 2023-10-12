using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Skill Cooltime Exclusive mask memo
// Link : https://www.youtube.com/watch?v=ZoiNP5IfBBo
// https://docs.unity3d.com/kr/2019.4/Manual/SL-Stencil.html
// https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/DefaultResourcesExtra/UI/UI-Default.shader
// Stencil Comparision : Comparison Operation
// Stencil Op : Stencil Teset를 통과하면 어떻게 처리할지를 결정
// Read mask : Stencil Buffer에서 값을 읽을 때 Mask
// Write Mask : Stencil Buffer에 값을 쓸 때 mask, Pixel에 해당되는 Mask가 아님에 유의
// Normal UI Sprite
// Stencil Comparision : 8(Always) - Always Render Pixel
// Stencil ID : 0
// Stencil Op : 0 (Keep)
// Stencil Write Mask : 255
// Stencil Read Mask : 255(255가 Default)
// Mask UI Sprite
// Stencil Comparision : 8(Always) - Always Render Pixel
// Stencil ID : 1
// Stencil Op : 2 (Replace) : Stencil Buffer 값 Replace
// Stencil Write Mask : 255
// Stencil Read Mask : 255
// Masked UI Sprite
// Stencil Comparision : 3(Equal) - 같을 경우에 Pixel render
// Stencil ID : 1
// Stencil Op : 0 (Keep)
// Stencil Write Mask : 0
// Stencil Read Mask : 1

// Mask 이미지가 Stencil Buffer에 Replace로 값을 쓴다.(Reference Value가 Stencil Buffer에 들어가게 된다.)
// Masked 이미지의 경우는 Comparision 연산으로 인해서 Ref ID가 1인 것들이 있을 경우에만 렌더링을 하게 된다.
// 이 때 Stencil OP는 Keep이기 때문에 따로 스텐실 버퍼를 사용하지 않는다.

namespace Titan.UI
{
    public class SkillIconController : MonoBehaviour
    {
        [Header("Skill Info")]
        // SerializeField for debug Info
        [SerializeField] private Sprite _iconSprite;
        /// <summary>
        /// Cooltime. [0, _cooltime], 0에서 _cooltime까지 올라간다. 쿨타임이 없을 수도 있으므로 0으로 설정 가능
        /// </summary>
        [SerializeField] private float _cooltime = 0;
        /// <summary>
        /// 현재 Cooltime, [0, _cooltime], 0에서 _cooltime까지 올라간다. 쿨타임이 없을 수도 있으므로 0으로 설정 가능
        /// </summary>
        [SerializeField] private float _curCooltime = 0;
        /// <summary>
        /// 현재 Energy
        /// </summary>
        [SerializeField] private float _curEnergy = 0;
        /// <summary>
        /// 최대 Energy, Energy를 사용하지 않는 스킬도 있으므로 0으로 설정 가능
        /// </summary>
        [SerializeField] private float _maxEnergy = 0;
        [SerializeField] private bool _isDisable = false;

        [Header("UI")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _cooltimeText;
        [SerializeField] private Slider _cooltimeSlider;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private Image _energyFillImage;

        [Header("Color")]
        [SerializeField] private Color _backgroundColor = Color.white;
        [SerializeField] private Color _iconColor = Color.white;
        [SerializeField] private Color _disableColor = Color.white;
        [SerializeField] private Color _energyFillColor = Color.white;
        [SerializeField] private Color _energyFullFillColor = Color.white;
        [SerializeField] private Color _cooltimeFillColor = Color.white;

        private bool IsValid => _cooltimeText != null && _iconImage != null;

        private void Awake()
        {
            if(_backgroundImage == null)
            {
                var backgroundGo = transform.Find("Background");
                if(backgroundGo != null)
                {
                    _backgroundImage = backgroundGo.GetComponent<Image>();
                }
            }

            if(_iconImage == null)
            {
                var skillImageGo = transform.Find("Background/SkillIcon");
                if(skillImageGo != null)
                {
                    _iconImage = skillImageGo.GetComponent<Image>();
                }
            }

            if(_cooltimeText == null)
            {
                var cooltimeTextGo = transform.Find("CooltimeText");
                if(cooltimeTextGo != null)
                {
                    _cooltimeText = cooltimeTextGo.GetComponent<TextMeshProUGUI>();
                }
            }

            if(_cooltimeSlider == null)
            {
                var cooltimeGo = transform.Find("CooltimeSlider");
                if(cooltimeGo != null)
                {
                    _cooltimeSlider = cooltimeGo.GetComponent<Slider>();
                }
            }

            if(_energySlider == null)
            {
                var energyGo = transform.Find("EnergySlider");
                if(energyGo != null)
                {
                    _energySlider = energyGo.GetComponent<Slider>();
                }

                if(_energySlider != null && _energySlider.fillRect != null
                    && _energySlider.fillRect.TryGetComponent<Image>(out var energyFillImage))
                {
                    _energyFillImage = energyFillImage;
                }
            }
        }

        private void Start()
        {
            if(!IsValid)
            {
                return;
            }

            if(_iconSprite != null)
            {
                _iconImage.sprite = _iconSprite;
            }
        }

        private void OnValidate()
        {
            // Update Color
            if(_backgroundImage != null)
            {
                _backgroundImage.color = _backgroundColor;
            }

            if(_iconImage != null)
            {
                _iconImage.color = _isDisable ? _iconColor : _disableColor;
            }

            if(_cooltimeSlider != null 
                && _cooltimeSlider.fillRect != null
                && _cooltimeSlider.fillRect.TryGetComponent<Image>(out var cooltimeFill))
            {
                cooltimeFill.color = _cooltimeFillColor;
            }
            
            if(_energySlider != null 
                && _energySlider.fillRect != null
                && _energySlider.fillRect.TryGetComponent<Image>(out var energyFill))
            {
                _energyFillImage = energyFill;
                energyFill.color = _energyFillColor;
            }

            UpdateCoolTime();
            UpdateEnergy();
            UpdateIcon();
        }

        /// <summary>
        /// Initialize Data
        /// </summary>
        /// <param name="skillIcon">표시할 Skill Icon</param>
        /// <param name="cooltime">Cooltime, CurCooltime은 Update UI 호출할 것</param>
        /// <param name="energy">MaxEnergy, CurEnery는 Update UI 호출할 것</param>
        public void InitSkillIcon(Sprite skillIcon, float cooltime, float energy)
        {
            _iconSprite = skillIcon != null ? skillIcon : _iconSprite;
            _cooltime = cooltime;
            _maxEnergy = energy;
        }

        public void UpdateUI(float curCooltime, float curEnergy)
        {
            if(!IsValid)
            {
                return;
            }

            _curCooltime = curCooltime;
            _curEnergy = curEnergy;

            UpdateCoolTime();
            UpdateEnergy();
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            if(_iconImage != null)
            {
                if(_isDisable)
                {
                    _iconImage.color = _disableColor;
                }
                else if(_curCooltime >= _cooltime && _curEnergy >= _maxEnergy)
                {
                    _iconImage.color = _iconColor;
                }
                else
                {
                    _iconImage.color = _disableColor;
                }
            }
        }

        private void UpdateCoolTime()
        {
            float remainTime = _cooltime - _curCooltime;
            remainTime = Mathf.Clamp(remainTime, 0f, _cooltime);

            if(_cooltimeText != null)
            {
                _cooltimeText.text = remainTime > 0f ? remainTime.ToString("F1") : string.Empty;
            }
            if(_cooltimeSlider != null)
            {
                _cooltimeSlider.value = remainTime > 0f ? remainTime / _cooltime : 0f;
            }
        }

        private void UpdateEnergy()
        {
            if(_energySlider != null)
            {
                _energySlider.value = _curEnergy / _maxEnergy;
            }
            if(_energyFillImage != null)
            {
                _energyFillImage.color = _energySlider.value >= 1f ? _energyFullFillColor : _energyFillColor;
            }
        }
    }
}

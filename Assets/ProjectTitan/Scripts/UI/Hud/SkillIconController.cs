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
        [SerializeField] private Sprite _skillIcon;
        [SerializeField] private float _cooltime;
        /// <summary>
        /// 현재 Cooltime, [0, _cooltime], 0에서 _cooltime까지 올라간다. 쿨타임이 없을 수도 있으므로 0으로 설정 가능
        /// </summary>
        [SerializeField] private float _curCooltime;
        /// <summary>
        /// 현재 Energy
        /// </summary>
        [SerializeField] private float _curEnergy;
        /// <summary>
        /// 최대 Energy, Energy를 사용하지 않는 스킬도 있으므로 0으로 설정 가능
        /// </summary>
        [SerializeField] private float _maxEnergy;

        [Header("UI")]
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _cooltimeText;
        [SerializeField] private Slider _cooltimeSlider;
        [SerializeField] private Slider _energySlider;


        private bool IsValid => _cooltimeText != null && _skillImage != null;

        private void Awake()
        {
            if(_cooltimeText == null)
            {
                var cooltimeTextGo = transform.Find("Background/CooltimeText");
                if(cooltimeTextGo != null)
                {
                    _cooltimeText = cooltimeTextGo.GetComponent<TextMeshProUGUI>();
                }
            }

            if(_skillImage == null)
            {
                var skillImageGo = transform.Find("Background/SkillIcon");
                if(skillImageGo != null)
                {
                    _skillImage = skillImageGo.GetComponent<Image>();
                }
            }
        }

        private void Start()
        {
            if(!IsValid)
            {
                return;
            }

            if(_skillIcon != null)
            {
                _skillImage.sprite = _skillIcon;
            }
        }

        public void InitSkillIcon(Sprite skillIcon, float cooltime, float energy)
        {
            _skillIcon = skillIcon != null ? skillIcon : _skillIcon;
            _cooltime = cooltime;
            _curEnergy = energy;
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
        }

        private void UpdateCoolTime()
        {
            float remainTime = _cooltime - _curCooltime;
            remainTime = Mathf.Clamp(remainTime, 0f, _cooltime);
            _cooltimeText.text = remainTime > 0f ? remainTime.ToString("F1") : string.Empty;
        }

        private void UpdateEnergy()
        {
            
        }
    }
}

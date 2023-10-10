using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Titan
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
            _skillIcon = skillIcon;
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

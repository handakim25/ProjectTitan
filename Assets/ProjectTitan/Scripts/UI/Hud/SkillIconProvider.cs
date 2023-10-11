using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    public class SkillIconProvider : MonoBehaviour
    {
        SkillIconController _controller;

        // @Refactor
        // 비슷하게 계속 사용되는 데이터의 묶음이 있다.
        // 추후에 모아서 한 번에 관리할 수 있도록 한다.
        [SerializeField] private Sprite _skillIcon = null;
        [SerializeField] private float _curCooltime = 0.0f;
        [SerializeField] private float _coolTime = 10.0f;
        [SerializeField] private float _curEnergy = 0.0f;
        [SerializeField] private float _energy = 40f;

        [SerializeField] private bool _updateCooltime = false;
        [SerializeField] private bool _regenEnergy = false;

        private void Awake()
        {
            _controller = GetComponent<SkillIconController>();
        }

        private void Start()
        {
            _controller.InitSkillIcon(_skillIcon, _coolTime, _energy);
        }

        private void Update()
        {
            _controller.UpdateUI(_curCooltime, _curEnergy);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Character.Player;

namespace Titan.UI
{
    public class SkillPannelController : MonoBehaviour
    {
        [SerializeField] private SkillIconController _basicIcon;
        [SerializeField] private SkillIconController _skillIcon;
        [SerializeField] private SkillIconController _hyperIcon;

        public void InitSkillData(PlayerStatus status)
        {
            if(_basicIcon != null)
            {
                _basicIcon.InitSkillIcon(status.BasicIcon, status.BasicCooltime, 0f);
            }
            if(_skillIcon != null)
            {
                _skillIcon.InitSkillIcon(status.SkillIcon, status.SkillCooltime, 0f);
            }
            if(_hyperIcon != null)
            {
                _hyperIcon.InitSkillIcon(status.HyperIcon, status.HyperCooltime, status.MaxEnergy);
            }
        }

        public void UpdateSkillData(PlayerStatus status)
        {
            // 현재 궁극기만 Energy를 가지고 있다.
            if(_basicIcon != null)
            {
                _basicIcon.UpdateUI(status.BasicCurCooltime, 0f);
            }
            if(_skillIcon != null)
            {
                _skillIcon.UpdateUI(status.SkillCurCooltime, 0f);
            }
            if(_hyperIcon != null)
            {
                _hyperIcon.UpdateUI(status.HyperCurCooltime, status.CurEnergy);
            }
        }
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Audio;

namespace Titan.Character
{
    /// <summary>
    /// Handle Footstep Event from animation clip
    /// </summary>
    public class FootStep : MonoBehaviour
    {
        [SerializeField] private SoundList[] _footStepSounds;
        [SerializeField] private SoundList _landSound;

        private int _footStepIndex = 0;

        private void Start()
        {
            _footStepSounds = _footStepSounds.Where(x => x != SoundList.None).ToArray();
        }

        void FootR()
        {
            PlayFootStep();
        }

        void FootL()
        {
            PlayFootStep();
        }

        private void PlayFootStep()
        {
            var curFootStep = GetFootStepSound();
            SoundManager.Instance.PlayOneShotEffect((int)curFootStep, transform.position, 1f);
        }

        private SoundList GetFootStepSound()
        {
            if(_footStepSounds.Length == 0)
            {
                return SoundList.None;
            }

            var soundList = _footStepSounds[_footStepIndex];
            _footStepIndex = (_footStepIndex + 1) % _footStepSounds.Length;
            return soundList;
        }

        void Land()
        {
            if(_landSound == SoundList.None)
            {
                return;
            }

            SoundManager.Instance.PlayEffectSound((int)_landSound);
        }
    }
}

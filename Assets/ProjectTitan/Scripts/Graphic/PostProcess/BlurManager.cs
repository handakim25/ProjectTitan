using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using Titan.Core;

namespace Titan.Graphics.PostProcessing
{
    [RequireComponent(typeof(Volume))]
    sealed public class BlurManager : MonoSingleton<BlurManager>
    {
        [SerializeField] BlurSettings _settings;
        private float _defaultStrength;
        
        private void Start()
        {
            // @Note
            // Render Pass에서 사용한 방법대로 하면 제대로 동작 안 한다.
            // _settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
            // 추정되는 이유는 stack을 제대로 가져오지 못하는 것 같다.
            var volume = GetComponent<Volume>();
            volume.profile.TryGet(out _settings);
            if(_settings != null)
            {
                _defaultStrength = _settings.Strength.value;
            }
        }

        public bool BlurActive
        {
            get => _settings != null && _settings.IsActive();
            set
            {
                if (_settings != null)
                {
                    _settings.active = value;
                }
            }
        }

        public float Strength
        {
            get => _settings != null ? _settings.Strength.value : 0f;
            set
            {
                if (_settings != null)
                {
                    _settings.Strength.value = value;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    public class BlurManager : MonoBehaviour
    {
        BlurSettings _settings;
        private float _defaultStrength;
        
        private void Start()
        {
            _settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
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

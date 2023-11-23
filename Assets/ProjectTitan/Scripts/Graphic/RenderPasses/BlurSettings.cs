using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    [System.Serializable, VolumeComponentMenu("Post-processing/Blur")]
    public class BlurSettings : VolumeComponent, IPostProcessComponent
    {
        // Slider 형식이다.
        [Tooltip("Standard deviation (Spread) of the blur.")]
        public ClampedFloatParameter Strength = new ClampedFloatParameter(0f, 0f, 15.0f);
        public ClampedIntParameter DownSample = new ClampedIntParameter(1, 1, 4);

        public bool IsActive()
        {
            return (Strength.value > 0f) && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

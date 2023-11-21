using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    public class BlurRenderFeature : ScriptableRendererFeature
    {
        BlurRenderPass _pass;

        public override void Create()
        {
            _pass = new();
            name = "Blur";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(_pass.Setup(renderer))
            {
                renderer.EnqueuePass(_pass);
            }

        }

    }
}

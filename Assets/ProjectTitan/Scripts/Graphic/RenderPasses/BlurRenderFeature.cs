using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    // Doc : https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.0/api/UnityEngine.Rendering.Universal.ScriptableRendererFeature.html
    public class BlurRenderFeature : ScriptableRendererFeature
    {
        BlurRenderPass _pass;

        public override void Create()
        {
            _pass = new();
            name = "Blur";
        }

        // 매 프레임마다 호출된다.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(_pass.Setup(renderer))
            {
                // @note
                // https://issuetracker.unity3d.com/issues/incorrect-rendering-when-intermediate-texture-is-set-to-auto
                // _pass.ConfigureInput(ScriptableRenderPassInput.Color);
                renderer.EnqueuePass(_pass);
            }

        }

    }
}

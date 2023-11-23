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
        [Tooltip("Blur에 사용할 Material, null일 경우 Titan/Blur Shader를 사용한다.")]
        [SerializeField] private Material _blurMat;
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        public override void Create()
        {
            _pass = new(_blurMat);
            name = "Blur";
        }

        // 매 프레임마다 호출된다.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(_pass.Setup(renderer))
            {
                _pass.BlurMat = _blurMat;
                _pass.renderPassEvent = _renderPassEvent;
                // @note
                // https://issuetracker.unity3d.com/issues/incorrect-rendering-when-intermediate-texture-is-set-to-auto
                // _pass.ConfigureInput(ScriptableRenderPassInput.Color);
                renderer.EnqueuePass(_pass);
            }
        }

    }
}

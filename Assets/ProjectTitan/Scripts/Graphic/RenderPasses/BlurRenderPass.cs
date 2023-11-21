using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    // Reference : https://www.youtube.com/watch?v=AlCuc58z7E8&t
    // Document : https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/api/UnityEngine.Rendering.Universal.ScriptableRenderPass.html
    public class BlurRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private BlurSettings _settings;

        private RenderTargetIdentifier _source;
        private RenderTargetHandle _blurTex;
        private int _blurTexID;

        private bool IsActive => _settings != null && _settings.IsActive();

        public bool Setup(ScriptableRenderer renderer)
        {
            // Camera Output Texture를 가져온다.
            _settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            if(IsActive)
            {
                // @Memo
                // 매 프레임마다 Material을 생성하는 것은 비효율적이다.
                _material = new Material(Shader.Find("Titan/Blur"));
                return true;
            }

            return false;
        }

        // @note
        // Configure : Render Pass를 실행하기전에 호출, render target이나 clearstate를 설정하거나 임시 렌더 텍스처를 만드는 등의 작업을 수행
        // 만약 오버라이드를 하지 않을 경우 활성화된 카메라된 카메라에 렌더링을 한다.
        // OnCameraSetup : 카메라를 렌더링하기 전에 호출. 하는 역할은 Configure와 동일하다.
        // https://qiita.com/ScreenPocket/items/c5e6f5d8959e22b61522 링크를 확인할 것
        // 카메라 Setup이 제일 밖에서 수행되고 각 렌더링마다 Configure가 수행된다.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if(!IsActive)
            {
                return;
            }
            Debug.Log($"On Camera Setup");

            // @Note
            // Setup에서 호출하면 제대로 작동하지 않는다.
            var renderer = renderingData.cameraData.renderer;
            _source = renderer.cameraColorTarget;

            var blurTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blurTargetDescriptor.depthBufferBits = 0;
            int width = blurTargetDescriptor.width;
            int height = blurTargetDescriptor.height;

            _blurTexID = Shader.PropertyToID("_BlurTex");
            _blurTex = new RenderTargetHandle
            {
                id = _blurTexID
            };

            // cmd.GetTemporaryRT(_blurTex.id, blurTargetDescriptor);

            cmd.GetTemporaryRT(_blurTex.id, width, height, 0, FilterMode.Trilinear);
        }

        // @optimize
        // Gausiaan 함수를 직접 구현하는 것보다는 미리 계산된 값을 가져오는 것이 더 빠르다.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(!IsActive)
            {
                return;
            }
            Debug.Log($"Execute");
            Debug.Log($"Source : {_source}");

            // Command Buffer Pool 이름은 Profiler에 잡힌다.
            CommandBuffer cmd = CommandBufferPool.Get("Blur Post Process");

            int gridSize = Mathf.CeilToInt(_settings.Strength.value * 3.0f);
            if(gridSize % 2 == 0)
            {
                gridSize++;
            }
            _material.SetInteger("_GridSize", gridSize);
            _material.SetFloat("_Spread", _settings.Strength.value);
            Debug.Log($"GridSize : {gridSize}, Spread : {_settings.Strength.value}");

            // Execute Effect using effect material with two passes
            // Pass 0 : Horizontal Blur
            // Pass 1 : Vertical Blur
            Blit(cmd, _source, _blurTex.id, _material, 0);
            // Blit(cmd, _blurTex.id, _source, _material, 1);
            Blit(cmd, _blurTex.id, _source);
            Debug.Log($"Material : {_material}");

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if(!IsActive)
            {
                return;
            }
            Debug.Log($"On Camera Cleanup");
            cmd.ReleaseTemporaryRT(_blurTexID);
        }
    }
}

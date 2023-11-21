using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    // Reference : https://www.youtube.com/watch?v=AlCuc58z7E8&t
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
            _source = renderer.cameraColorTarget;
            _settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            if(IsActive)
            {
                _material = new Material(Shader.Find("Titan/Blur"));
                return true;
            }

            return false;
        }

        // Effect를 적용하기 전에 호출된다.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if(!IsActive)
            {
                return;
            }

            _blurTexID = Shader.PropertyToID("_BlurTex");
            _blurTex = new RenderTargetHandle
            {
                id = _blurTexID
            };
            cmd.GetTemporaryRT(_blurTex.id, cameraTextureDescriptor);

            base.Configure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(!IsActive)
            {
                return;
            }

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
            Blit(cmd, _blurTex.id, _source, _material, 1);
            Debug.Log($"Material : {_material}");
            

            context.ExecuteCommandBuffer(cmd);
            // cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_blurTexID);
            base.FrameCleanup(cmd);
        }
    }
}

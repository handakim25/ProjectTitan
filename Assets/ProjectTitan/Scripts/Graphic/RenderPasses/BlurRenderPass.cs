using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Titan.Graphics.PostProcessing
{
    // Reference : https://www.youtube.com/watch?v=AlCuc58z7E8&t
    // Document : https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/api/UnityEngine.Rendering.Universal.ScriptableRenderPass.html
    public class BlurRenderPass : ScriptableRenderPass
    {
        private Material _blurMat;
        private BlurSettings _settings;
        public Material BlurMat
        {
            get => _blurMat;
            set 
            {
                _blurMat = value;
                if(_blurMat == null)
                {
                    _blurMat = CoreUtils.CreateEngineMaterial(_defaultShaderName);
                }
            }
        }

        private RenderTargetIdentifier _source;
        private RenderTargetHandle _blurTex;
        private int _blurTexID;

        private bool IsActive => _settings != null && _settings.IsActive();
        readonly string _profilerTag = "Blur Post Process";
        readonly string _defaultShaderName = "Titan/Blur";

        public BlurRenderPass(Material blurMat = null)
        {
            BlurMat = blurMat;
        }

        public bool Setup(ScriptableRenderer renderer)
        {
            _settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();

            // OnCameraSetup 주석 확인할 것
            // _source = renderer.cameraColorTarget;
            // Debug.Log($"Setup Source : {_source}");
            // Type CameraTarget NameID -1 InstanceID 0 BufferPointer 0 MipLevel 0 CubeFace Unknown DepthSlice 0

            return IsActive;
        }

        // @note
        // Configure : Render Pass를 실행하기전에 호출, render target이나 clearstate를 설정하거나 임시 렌더 텍스처를 만드는 등의 작업을 수행
        // 만약 오버라이드를 하지 않을 경우 활성화된 카메라된 카메라에 렌더링을 한다.
        // OnCameraSetup : 카메라를 렌더링하기 전에 호출. 하는 역할은 Configure와 동일하다.
        // https://qiita.com/ScreenPocket/items/c5e6f5d8959e22b61522 링크를 확인할 것
        // 카메라 OnCameraSetup이 제일 밖에서 수행되고 각 렌더링마다 Configure가 수행된다.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if(!IsActive)
            {
                return;
            }

            // @Note
            // Setup에서 호출하면 제대로 작동하지 않는다.
            // Intermediate Texture가 Always일 경우에는 Setup에서 호출해도 동작한다.
            // https://issuetracker.unity3d.com/issues/incorrect-rendering-when-intermediate-texture-is-set-to-auto
            // 해당 링크에 따르면 2021.2 이후에서는 Always가 기본값이고
            // 2022.1에서는 SetupRenderPasses를 지원한다.
            var renderer = renderingData.cameraData.renderer;
            _source = renderer.cameraColorTarget;
            // source :  Type PropertyName NameID 1615 InstanceID 0 BufferPointer 0 MipLevel 0 CubeFace Unknown DepthSlice -1
            // Debug.Log($"OnCameraSetup Source : {_source}");

            var blurTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            // @To-Do : Downsample을 설정할 수 있도록 한다.
            int width = blurTargetDescriptor.width / _settings.DownSample.value;
            int height = blurTargetDescriptor.height / _settings.DownSample.value;

            _blurTexID = Shader.PropertyToID("_BlurTex");
            _blurTex = new RenderTargetHandle
            {
                id = _blurTexID
            };

            cmd.GetTemporaryRT(_blurTex.id, width, height, 0, FilterMode.Trilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(!IsActive)
            {
                return;
            }
            if(_blurMat == null)
            {
                Debug.LogWarning($"Blur Material is null");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

            // @optimize
            // Gausiaan 함수를 직접 구현하는 것보다는 미리 계산된 값을 가져오는 것이 더 빠르다.
            int gridSize = Mathf.CeilToInt(_settings.Strength.value * 3.0f);
            if(gridSize % 2 == 0)
            {
                gridSize++;
            }
            _blurMat.SetInteger("_GridSize", gridSize);
            _blurMat.SetFloat("_Spread", _settings.Strength.value);

            // Pass 0 : Horizontal Blur
            // Pass 1 : Vertical Blur
            Blit(cmd, _source, _blurTex.id, _blurMat, 0);
            Blit(cmd, _blurTex.id, _source, _blurMat, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if(!IsActive)
            {
                return;
            }
            cmd.ReleaseTemporaryRT(_blurTexID);
        }
    }
}

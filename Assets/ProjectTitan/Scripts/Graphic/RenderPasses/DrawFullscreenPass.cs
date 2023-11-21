namespace UnityEngine.Rendering.Universal
{
    // Reference : Unity URP Shader Graph
    // Reference : https://github.com/Unity-Technologies/Graphics/blob/9511b1d437f592df72692c1ea2b811cee4186d01/Packages/com.unity.render-pipelines.universal/Runtime/Passes/ScriptableRenderPass.cs

    /// <summary>
    /// Draws full screen mesh using given material and pass and reading from source target.
    /// </summary>
    internal class DrawFullscreenPass : ScriptableRenderPass
    {
        public FilterMode FilterMode { get; set; }
        public DrawFullscreenFeature.Settings Settings;

        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;
        int temporaryRTId = Shader.PropertyToID("_TempRT");
        // public int downsample;

        int sourceId;
        int destinationId;
        bool isSourceAndDestinationSameTarget;

        string _profilerTag;

        public DrawFullscreenPass(string tag)
        {
            _profilerTag = tag;
        }

        // OnCameraSetUP
        // 카메라를 호출하기 전에 호출된다.
        // Render Target을 설정하거나 Clear State를 설정하거나, 임시 Render Target을 생성하는 등의 작업을 한다.
        // 만약, Render Pass가 이 메소드를 상속받지 않을 경우, Render Pass는 현재 활성화된 카메라의 Render Target에 렌더링한다.
        // CommandBuffer.SetRenderTarget를 호출해서는 안된다. 대신, ConfigureTarget과 ConfigureClear를 호출한다.

        /// <summary>
        /// Source Render Target과 Destination Render Target을 설정한다.
        /// </summary>
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            int width = blitTargetDescriptor.width / Settings.downsample;
            int height = blitTargetDescriptor.height / Settings.downsample;

            isSourceAndDestinationSameTarget = Settings.sourceType == Settings.destinationType &&
                (Settings.sourceType == BufferType.CameraColor || Settings.sourceTextureId == Settings.destinationTextureId);

            var renderer = renderingData.cameraData.renderer;

            if (Settings.sourceType == BufferType.CameraColor)
            {
                sourceId = -1;
                source = renderer.cameraColorTarget;
            }
            else
            {
                sourceId = Shader.PropertyToID(Settings.sourceTextureId);
                // cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, filterMode);
                cmd.GetTemporaryRT(sourceId, width, height, 0, FilterMode.Trilinear, RenderTextureFormat.ARGBHalf);
                source = new RenderTargetIdentifier(sourceId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                // Source와 Destination이 같을 경우
                // Destination을 임시 Render Target으로 설정한다.

                destinationId = temporaryRTId;
                // cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                cmd.GetTemporaryRT(destinationId, width, height, 0, FilterMode.Trilinear, RenderTextureFormat.ARGBHalf);
                destination = new RenderTargetIdentifier(destinationId);
            }
            else if (Settings.destinationType == BufferType.CameraColor)
            {
                destinationId = -1;
                destination = renderer.cameraColorTarget;
            }
            else
            {
                destinationId = Shader.PropertyToID(Settings.destinationTextureId);
                // cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                cmd.GetTemporaryRT(destinationId, width, height, 0, FilterMode.Trilinear, RenderTextureFormat.ARGBHalf);
                destination = new RenderTargetIdentifier(destinationId);
            }
        }

        /// <summary>
        /// Pass를 실행한다. 실제 렌더링은 여기서 이루어진다. 구체적인 내용은 구현에 맡긴다.
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Command Buffer에 Command를 기록하고 한 번에 수행하는 듯 하다.
            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);


            // Blit(Bit Block Transfer) : 한 Render Target에서 다른 Render Target으로 텍스처를 복사하는 것
            // cmd : Command를 기록할 Command Buffer
            // source : Source Texture 혹은 Target Identifier. Blit froim
            // destination : Destination Texture 혹은 Target Identifier. Blit to
            // material : Blit에 사용할 Material
            // passIndex : Shader Pass에 사용할 Index, 기본값 0

            // Can't read and write to same color target, create a temp render target to blit. 
            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, source, destination, Settings.blitMaterial, Settings.blitMaterialPassIndex);
                Blit(cmd, destination, source);
            }
            else
            {
                Blit(cmd, source, destination, Settings.blitMaterial, Settings.blitMaterialPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }
    }
}

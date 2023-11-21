namespace UnityEngine.Rendering.Universal
{
    // Reference : Unity URP Shader Graph
    // Reference : https://www.youtube.com/watch?v=AlCuc58z7E8&t=280s
    // Reference : https://www.youtube.com/watch?v=JF4t9pNaZxg
    public enum BufferType
    {
        CameraColor,
        Custom 
    }

    // Rendering Pipeline에 추가할 Feature Class이다.
    // 실제 작동은 RenderPass에서 이루어지며, Feature는 RenderPass를 생성하고, RenderPipeline에 추가하는 역할을 한다.
    public class DrawFullscreenFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            [Range(1,4)]
            public int downsample = 1;
            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
            public BufferType sourceType = BufferType.CameraColor;
            public BufferType destinationType = BufferType.CameraColor;
            public string sourceTextureId = "_SourceTexture";
            public string destinationTextureId = "_DestinationTexture";
        }

        public Settings settings = new Settings();
        DrawFullscreenPass blitPass;

        // Resource를 초기화하기 위한 함수
        // OnEnable, OnValidate에서 호출
        // Scriptable Object이므로 OnEnable 호출 시점은 프로젝트가 로드되거나, Inspector에서 해당 오브젝트를 선택했을 때
        public override void Create()
        {
            blitPass = new DrawFullscreenPass(name);
        }

        // 하나 혹은 여러개의 ScriptableRenderPass를 삽입한다.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing refe`rence in the assigned renderer.", GetType().Name);
                return;
            }

            blitPass.renderPassEvent = settings.renderPassEvent;
            blitPass.Settings = settings;
            renderer.EnqueuePass(blitPass);
        }
    }
}


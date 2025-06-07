using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelationSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        [Range(1, 128)]
        public int pixelSize = 8;
        public bool isActive = true;
    }

    public PixelationSettings settings = new PixelationSettings();
    private PixelationPass _pixelationPass;

    public override void Create()
    {
        _pixelationPass = new PixelationPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.isActive || settings.pixelSize <= 1 ||
            renderingData.cameraData.isPreviewCamera ||
            renderingData.cameraData.isSceneViewCamera ||
            renderingData.cameraData.cameraType == CameraType.Reflection) // 반사 카메라 등 제외
        {
            return;
        }

        // _pixelationPass.SetCameraTarget(renderer.cameraColorTargetHandle); // 이 줄을 제거하거나 주석 처리
        renderer.EnqueuePass(_pixelationPass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
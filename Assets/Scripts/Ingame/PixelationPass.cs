using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationPass : ScriptableRenderPass
{
    private PixelationFeature.PixelationSettings _settings;
    private RTHandle _cameraColorTargetHandle;
    private int _tempRTId;
    private string _profilerTag;

    public PixelationPass(PixelationFeature.PixelationSettings settings)
    {
        this.renderPassEvent = settings.renderPassEvent;
        _settings = settings;
        _profilerTag = "Pixelation Effect Pass";
        _tempRTId = Shader.PropertyToID("_PixelationTempTex");
    }

    // public void SetCameraTarget(RTHandle cameraColorTargetHandle) // 이 메서드 제거
    // {
    //     _cameraColorTargetHandle = cameraColorTargetHandle;
    // }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // renderingData에서 카메라 컬러 타겟 핸들을 가져옵니다.
        _cameraColorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if (_settings.pixelSize <= 1 || !_settings.isActive) return;

        RenderTextureDescriptor descriptor = cameraTextureDescriptor;
        descriptor.depthBufferBits = 0;

        int downscaledWidth = Mathf.Max(1, descriptor.width / _settings.pixelSize);
        int downscaledHeight = Mathf.Max(1, descriptor.height / _settings.pixelSize);

        descriptor.width = downscaledWidth;
        descriptor.height = downscaledHeight;

        cmd.GetTemporaryRT(_tempRTId, descriptor, FilterMode.Point);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_settings.pixelSize <= 1 || !_settings.isActive ||
            _cameraColorTargetHandle == null || _cameraColorTargetHandle.IsUnityNull())
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

        // OnCameraSetup에서 이미 _cameraColorTargetHandle이 할당되었으므로 바로 사용합니다.
        RTHandle sourceHandle = _cameraColorTargetHandle;
        RenderTargetIdentifier tempRTIdentifier = new RenderTargetIdentifier(_tempRTId);

        cmd.Blit(sourceHandle, tempRTIdentifier);
        cmd.Blit(tempRTIdentifier, sourceHandle);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException(nameof(cmd));

        if (_settings.pixelSize > 1 && _settings.isActive) // Configure에서 GetTemporaryRT를 호출한 경우에만 해제
        {
            cmd.ReleaseTemporaryRT(_tempRTId);
        }
    }
}
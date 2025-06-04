using UnityEngine;

[ExecuteInEditMode] // 에디터에서도 효과를 바로 볼 수 있게 합니다.
[RequireComponent(typeof(Camera))] // 이 스크립트는 카메라에만 부착될 수 있도록 합니다.
[AddComponentMenu("Effects/Pixelation")] // 컴포넌트 메뉴에 효과를 추가합니다.
public class PixelationEffect : MonoBehaviour
{
    [Range(1, 128)] // 픽셀 크기(블록의 한 변을 이루는 원본 픽셀 수)
    public int pixelSize = 8;

    private Material _passthroughMaterial; // 단순 패스스루 머티리얼 (Blit에 필요할 수 있음)

    void OnEnable()
    {
        // URP/HDRP에서는 OnRenderImage가 직접 호출되지 않으므로 다른 접근 방식이 필요합니다.
        // 이 코드는 주로 Built-in Render Pipeline 용입니다.
        // 패스스루 쉐이더가 필요한 경우 (예: 특정 Blit 오버로드)
        // _passthroughMaterial = new Material(Shader.Find("Hidden/Internal-BlitCopy")); // 간단한 복사 쉐이더
    }

    void OnDisable()
    {
        // if (_passthroughMaterial != null)
        // {
        //     DestroyImmediate(_passthroughMaterial);
        //     _passthroughMaterial = null;
        // }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelSize <= 1) // pixelSize가 1 이하면 효과 없음
        {
            Graphics.Blit(source, destination);
            return;
        }

        // 다운샘플링할 해상도 계산
        int downscaledWidth = source.width / pixelSize;
        int downscaledHeight = source.height / pixelSize;

        // 해상도가 너무 낮아지면 (0 이하) 효과 적용하지 않음
        if (downscaledWidth <= 0 || downscaledHeight <= 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // 낮은 해상도의 임시 렌더 텍스처 생성
        RenderTexture tempRT = RenderTexture.GetTemporary(downscaledWidth, downscaledHeight, 0, source.format);

        // 중요: 픽셀 효과를 위해 Point 필터링 사용
        tempRT.filterMode = FilterMode.Point;

        // 1. 원본 이미지를 낮은 해상도의 임시 텍스처로 복사 (다운샘플링)
        Graphics.Blit(source, tempRT);

        // 2. 낮은 해상도의 임시 텍스처를 최종 목적지 텍스처로 복사 (업샘플링)
        //    Point 필터링 덕분에 픽셀화된 효과가 나타남
        Graphics.Blit(tempRT, destination);

        // 임시 렌더 텍스처 해제
        RenderTexture.ReleaseTemporary(tempRT);
    }
}
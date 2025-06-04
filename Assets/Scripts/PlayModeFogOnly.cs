using UnityEngine;

public class PlayModeFogOnly : MonoBehaviour
{
    public bool applyCustomFogInPlayMode = true;

    [Header("Play Mode Fog Settings")]
    public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public FogMode fogMode = FogMode.ExponentialSquared;
    [Range(0.0001f, 0.5f)]
    public float fogDensity = 0.01f;
    public float linearFogStart = 20f;
    public float linearFogEnd = 100f;

    void Start()
    {
        if (applyCustomFogInPlayMode)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = this.fogColor;
            RenderSettings.fogMode = this.fogMode;

            if (this.fogMode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = this.linearFogStart;
                RenderSettings.fogEndDistance = this.linearFogEnd;
            }
            else
            {
                RenderSettings.fogDensity = this.fogDensity;
            }
        }
        else
        {
            RenderSettings.fog = false;
        }
    }
}
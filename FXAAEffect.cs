using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FXAAEffect : MonoBehaviour
{
    // 亮度计算方式
    public enum LuminanceMode { Alpha, Green, Calculate }

    public LuminanceMode luminanceSource;

    [Range(0f, 1f)]
    public float subpixelBlending = 1f;

    // 对比度阈值
    [Range(0.0312f, 0.0833f)]
    public float contrastThreshold = 0.0312f;

    // 相对阈值
    [Range(0.063f, 0.333f)]
    public float relativeThreshold = 0.063f;

    public Shader fxaaShader;

    public bool lowQuality;

    public bool gammaBlending;

    [NonSerialized]
    Material fxaaMaterial;

    const int luminancePass = 0;
    const int fxaaPass = 1;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (fxaaMaterial == null)
        {
            fxaaMaterial = new Material(fxaaShader);
            fxaaMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        fxaaMaterial.SetFloat("_ContrastThreshold", contrastThreshold);
        fxaaMaterial.SetFloat("_RelativeThreshold", relativeThreshold);
        fxaaMaterial.SetFloat("_SubpixelBlending", subpixelBlending);

        if (lowQuality)
        {
            fxaaMaterial.EnableKeyword("LOW_QUALITY");
        }
        else
        {
            fxaaMaterial.DisableKeyword("LOW_QUALITY");
        }

        if (gammaBlending)
        {
            fxaaMaterial.EnableKeyword("GAMMA_BLENDING");
        }
        else
        {
            fxaaMaterial.DisableKeyword("GAMMA_BLENDING");
        }



        // 切换亮度计算方式
        if (luminanceSource == LuminanceMode.Calculate)
        {
            fxaaMaterial.DisableKeyword("LUMINANCE_GREEN");

            RenderTexture luminanceTex = RenderTexture.GetTemporary(
                source.width, source.height, 0, source.format
            );
            Graphics.Blit(source, luminanceTex, fxaaMaterial, luminancePass);
            Graphics.Blit(luminanceTex, destination, fxaaMaterial, fxaaPass);
            RenderTexture.ReleaseTemporary(luminanceTex);
        }
        else //直接使用g/a通道作为亮度
        {
            if (luminanceSource == LuminanceMode.Green)
            {
                fxaaMaterial.EnableKeyword("LUMINANCE_GREEN");
            }
            else
            {
                fxaaMaterial.DisableKeyword("LUMINANCE_GREEN");
            }
            Graphics.Blit(source, destination, fxaaMaterial, fxaaPass);
        }


    }

}
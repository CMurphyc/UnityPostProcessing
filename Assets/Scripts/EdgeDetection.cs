using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : PostEffectsBase
{
    public Shader edgeDetectShader;
    private Material edgeDetectMat = null;
    public Material material
    {
        get
        {
            edgeDetectMat = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMat);
            return edgeDetectMat;
        }
    }

    [Range(0.0f, 1.0f)]
    public float edgeIntensity = 0.0f;
    public Color edgeColor = Color.black;
    public Color backgroundColor = Color.white;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (material != null)
        {
            material.SetFloat("_EdgeIntensity", edgeIntensity);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgroundColor);

            Graphics.Blit(src, dest, material);
        }
        else Graphics.Blit(src, dest);
    }
}

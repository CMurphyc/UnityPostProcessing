using UnityEngine;
using System.Collections;

public class GaussianBlur : PostEffectsBase 
{

	public Shader gaussianBlurShader;
	private Material gaussianBlurMaterial = null;

	public Material material 
	{  
		get 
		{
			gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
			return gaussianBlurMaterial;
		}  
	}
	public int iterations = 3;
	
	public float blurSpread = 0.6f;
	
	public int downSample = 2;
	
	void OnRenderImage (RenderTexture src, RenderTexture dest) 
	{
		if (material != null) 
		{
			int rtW = src.width / downSample;
			int rtH = src.height / downSample;

			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;

			Graphics.Blit(src, buffer0);

			for (int i = 0; i < iterations; i++) 
			{
				material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				Graphics.Blit(buffer0, buffer1, material, 0);

				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				Graphics.Blit(buffer0, buffer1, material, 1);

				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
			}

			Graphics.Blit(buffer0, dest);
			RenderTexture.ReleaseTemporary(buffer0);
		} 
		else Graphics.Blit(src, dest);
	}
}

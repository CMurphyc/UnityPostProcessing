using UnityEngine;
using System.Collections;

public class Bloom : PostEffectsBase {

	public Shader bloomShader;
	private Material bloomMaterial = null;
	public Material material 
    {  
		get 
        {
			bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
			return bloomMaterial;
		}  
	}

	public int iterCount = 5;
	public float blurSpread = 0.5f;

	public int downSample = 2;

	public float lumiThreshold = 0.6f;

	void OnRenderImage (RenderTexture src, RenderTexture dest) 
    {
		if (material != null) 
        {
			material.SetFloat("_LuminanceThreshold", lumiThreshold);

			int rtW = src.width/downSample;
			int rtH = src.height/downSample;
			
			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;
			
			Graphics.Blit(src, buffer0, material, 0);
			
			for (int i = 0; i < iterCount; i++) 
            {
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				material.SetFloat("_BlurSize", 1.0f + i * blurSpread);
								
				Graphics.Blit(buffer0, buffer1, material, 1);
				
				RenderTexture.ReleaseTemporary(buffer0);

				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
				Graphics.Blit(buffer0, buffer1, material, 2);
				
				RenderTexture.ReleaseTemporary(buffer0);

				buffer0 = buffer1;
			}

			material.SetTexture("_Bloom", buffer0);  
			Graphics.Blit(src, dest, material, 3);  

			RenderTexture.ReleaseTemporary(buffer0);
		} 
        else Graphics.Blit(src, dest);
	}
}

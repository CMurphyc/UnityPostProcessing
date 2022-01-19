Shader "Hidden/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeIntensity ("EdgeIntensity", Float) = 1.0
        _EdgeColor ("EdgeColor", Color) = (0, 0, 0, 1)
        _BackgroundColor ("BackgroundColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            fixed _EdgeIntensity;
            fixed4 _EdgeColor;
            fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        o.uv[(i + 1) * 3 + j + 1] = v.uv + _MainTex_TexelSize * half2(i, j);
                    }
                }
                return o;
            }

            fixed luminance (fixed4 color)
            {
                return color.r * 0.2125 + color.g * 0.7154 + color.b * 0.0721;
            }

            half Sobel(v2f i)
            {
                const half Gx[9] = {
                    -1, -2, -1,
                    0, 0, 0,
                    1, 2, 1
                };
                const half Gy[9] = {
                    -1, 0, 1,
                    -2, 0, 2,
                    -1, 0, 1
                };

                half edgeX = 0;
                half edgeY = 0;
                for (int index = 0; index < 9; index++)
                {
                    half lumi = luminance(tex2D(_MainTex, i.uv[index]));
                    edgeX += lumi * Gx[index];
                    edgeY += lumi * Gy[index];

                }
                return 1 - abs(edgeX) -abs(edgeY);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half edgeG = Sobel(i);

                fixed4 withoutMainTexColor = lerp(_EdgeColor, _BackgroundColor, edgeG);
                fixed4 withMainTexColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[4]), edgeG);

                return lerp(withMainTexColor, withoutMainTexColor, _EdgeIntensity);
            }
            ENDCG
        }
    }
    Fallback Off
}

Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        _OutlineStrength ("Outline Strength", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGINCLUDE
        #include "UnityCG.cginc"
        #include "UnityLightingCommon.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };
            
        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 screenPos : TEXCOORD0;
            float3 worldNormal : TEXCOORD1;
            float eyeDepth : TEXCOORD2;
        };

        sampler2D _CameraDepthTexture;
        float4 _MainColor;
        float4 _OutlineColor;
        float _OutlineWidth;
        float _OutlineStrength;
        float4 _CameraDepthTexture_TexelSize;
        
        v2f vert(appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.screenPos = ComputeScreenPos(o.pos);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.eyeDepth = -UnityObjectToViewPos(v.vertex).z;
            return o;
        }
        ENDCG

        // Pass 1: 渲染物体本身
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                // 计算基础光照
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = dot(worldNormal, worldLight) * 0.5 + 0.5;
                float3 diffuse = _LightColor0.rgb * _MainColor.rgb * ndotl;
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                return float4(ambient + diffuse, 1);
            }
            ENDCG
        }

        // Pass 2: 渲染描边
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float SampleDepth(float2 uv)
            {
                return LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float depth = i.eyeDepth;
                
                float outline = 0;
                float2 pixelSize = _CameraDepthTexture_TexelSize.xy * _OutlineWidth;

                // 采样周围像素的深度
                float d1 = SampleDepth(screenUV + float2(-pixelSize.x, 0));
                float d2 = SampleDepth(screenUV + float2(pixelSize.x, 0));
                float d3 = SampleDepth(screenUV + float2(0, -pixelSize.y));
                float d4 = SampleDepth(screenUV + float2(0, pixelSize.y));

                // 计算深度差异
                float d = max(
                    max(abs(d1 - depth), abs(d2 - depth)),
                    max(abs(d3 - depth), abs(d4 - depth))
                );

                // 如果深度差异大于阈值，则认为是边缘
                if (d > 0.01)
                {
                    outline = _OutlineStrength;
                }

                return float4(_OutlineColor.rgb, outline);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
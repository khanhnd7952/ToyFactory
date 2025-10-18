Shader "BrickOut/3D/Holes"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_ShadowColor ("Shadow Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			CBUFFER_START(UnityPerMaterial)
				half4 _Color,_ShadowColor;
			CBUFFER_END
			half _DarkGlobal;
		

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv.xy = v.uv;
				o.uv.z = mul(unity_ObjectToWorld,v.vertex).y;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				
                half ao = tex2D(_MainTex, i.uv.xy).r;
            	half4 color = _Color;
            	if (_Color.r+_Color.g+_Color.b<0.9)
				{
					color = _Color+0.1;
				}
				half4 final = lerp(_ShadowColor,color*1.1,ao);
				
				if(i.uv.z<0)
				{
					half3 dark = half3(0.06,0.14,0.31);
					if (_Color.r+_Color.g+_Color.b<0.9)
					{
						dark = 0.1;
					}
					half darkHole = saturate(i.uv.z*0.15+1);
					final.rgb=lerp(final.rgb,dark,0.3);
					final.rgb = lerp(dark,final.rgb,darkHole);
				}
				final.a = 1;
				final.rgb *= (1 - _DarkGlobal);
                return final;
            }
            ENDHLSL
        }
    }
}

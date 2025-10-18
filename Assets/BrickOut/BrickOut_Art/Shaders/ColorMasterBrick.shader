Shader "BrickOut/3D/ColorMasterBrick"
{
    Properties
    {
        [NoScaleOffset]_AO ("Ambient Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_ShadowColor ("Shadow Color", Color) = (0,0,0,1)
		[Header(Brick Settings)]
		[Toggle(BRICK_TYPE)] _BrickType("Brick Type", Int) = 0
		_BrickHeight("Brick Height", Float) = 1
		[Toggle]_IsModel("Is Model", Int) = 0
    }
    SubShader
    {
		HLSLINCLUDE
		
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		
		CBUFFER_START(UnityPerMaterial)
			half4 _Color,_ShadowColor;
			half _BrickHeight;
			int _IsModel;
		CBUFFER_END
		
		ENDHLSL
        Pass
        {
			Blend One OneMinusSrcAlpha
			
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature BRICK_TYPE
            
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "LightingCustom.hlsl"
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _AO;
			
			

            v2f vert (appdata v)
            {
                v2f o;
				#ifdef BRICK_TYPE
					if(v.vertex.y>0.5)
					{
						v.vertex.y +=_BrickHeight-1;
					}
				#endif
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
				o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half ao = tex2D(_AO, i.uv).x;
				half4 final = 1;
				final.a = saturate(i.worldPos.y*0.15+1);
				//
				half3 lightDirection;
				half shadowAtten;
				
				if(_IsModel == 0)
				{
					half4 shadowCoord = TransformWorldToShadowCoord(i.worldPos);
					Light mainLight = GetMainLight(shadowCoord);
					lightDirection = mainLight.direction;
					shadowAtten = mainLight.shadowAttenuation;
				}
				else
				{
					lightDirection = half3(0.08, 0.19, -0.98);
					shadowAtten = 1;
				}
				final.rgb = CaculateLighting(_Color.rgb,_ShadowColor,ao,i.worldNormal,lightDirection,shadowAtten);
				//
				half spec = Specular(lightDirection, i.worldNormal) * shadowAtten;
				half3 colSpec = lerp(saturate(final.rgb*3),1,_Color.a*2);
				if (_IsModel != 0)
				{
					spec *= 0.65;
				}
				
				final.rgb = lerp(final.rgb,colSpec,spec);

            	if (_IsModel == 0)
				{
            		final.rgb *= final.a;
					//final.a = 1;
				}
	            else
	            {
		            final.a = pow(final.a,2.25);
	            	final.rgb *= final.a;
	            }
            	
				
                return final;
            }
            ENDHLSL
        }
		Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowPassVertexModify
            #pragma fragment ShadowPassFragment
			#pragma shader_feature BRICK_TYPE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"


            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
			
			Varyings ShadowPassVertexModify(Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				#ifdef BRICK_TYPE
					if(input.positionOS.y>0.5)
					{
						input.positionOS.y+=_BrickHeight-1;
					}
				#endif

				output.positionCS = GetShadowPositionHClip(input);
				return output;
			}
			
            ENDHLSL
        }
    }
}

Shader "Fred/Sprites/sdf-splitRGB-colourable"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_SDFDistance ("SDF Distance (default 16)", Float) = 16
		_Softness ("Softness", Range(0,1)) = 0
		[KeywordEnum(combined, sequencial)] _AlphaRGBmult ("Sprite alpha controls sdf offset", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 2.0

			#pragma multi_compile __ _ALPHARGBMULT_COMBINED _ALPHARGBMULT_SEQUENCIAL
			#include "UnitySprites.cginc"

			struct simplev2f
			{
			    float4 vertex   : SV_POSITION;
			    fixed4 color    : COLOR;
			    float2 texcoord : TEXCOORD0;
			};

			simplev2f Vert(appdata_t IN)
			{
			    simplev2f OUT;
			    OUT.vertex = UnityObjectToClipPos(IN.vertex);
			    OUT.texcoord = IN.texcoord;
			    OUT.color = IN.color * _RendererColor;

			    return OUT;
			}

			float4 _MainTex_TexelSize;
			fixed _Softness;
			fixed _SDFDistance;

			fixed4 Frag(simplev2f IN) : SV_Target
			{
				fixed4 color = IN.color;
				float4 sdf = tex2D(_MainTex, IN.texcoord);
				//float4 sdfShadow = tex2D(_MainTex, IN.texcoord1);

				//setup antialiazing
				float dx = ddx(IN.texcoord.x) * _MainTex_TexelSize.z;
				float dy = ddy(IN.texcoord.y) * _MainTex_TexelSize.w;

				//_SDFDistance is the distance of the field. Could be controlled by blue channel.
				float factor = _SDFDistance *(1-_Softness);
				float toPixels = max(1,factor * rsqrt(dx*dx+dy*dy));

				#ifdef _ALPHARGBMULT_COMBINED
					//convert sdf to antialiased solid shape
					sdf = saturate((sdf-0.5) *toPixels +0.5);
					color.a *= saturate(sdf.r + sdf.g + sdf.b);
				#else
					#ifdef _ALPHARGBMULT_SEQUENCIAL
						sdf.rgb *= min(1,float3(color.a*3, color.a*3-1,color.a*3-2));
						sdf = saturate((sdf-0.5) *toPixels +0.5);
						color.a = saturate(sdf.r + sdf.g + sdf.b);
					#else
						sdf = saturate((sdf-0.5) *toPixels +0.5);
						color.a *= saturate(sdf.r + sdf.g + sdf.b);
					#endif
				#endif

				color.rgb *= color.a;

				return color;
			}

		ENDCG
		}
	}
}

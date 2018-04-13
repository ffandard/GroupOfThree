Shader "Fred/Sprites/sdf-splitRGB-colourable-shadowed"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_SDFDistance ("SDF Distance (default 16)", Float) = 16
		_Softness ("Softness", Range(0,1)) = 0
		_ShadowDistance ("Shadow Blur", Range(0,1)) = 0
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

			#include "UnitySprites.cginc"

			struct simplev2f
			{
			    float4 vertex   : SV_POSITION;
			    fixed4 color    : COLOR;
			    float2 texcoord : TEXCOORD0;
			    float2 texcoord1 : TEXCOORD1;
			};

			simplev2f Vert(appdata_t IN)
			{
			    simplev2f OUT;
			    OUT.vertex = UnityObjectToClipPos(IN.vertex);
			    OUT.texcoord = IN.texcoord;
			    OUT.texcoord1 = IN.texcoord + float2(-0.02*(1-IN.color.a),0.05*(1-IN.color.a));
			    OUT.color = IN.color * _RendererColor;

			    return OUT;
			}

			float4 _MainTex_TexelSize;
			fixed _Softness;
			fixed _ShadowDistance;
			fixed _SDFDistance;

			fixed4 Frag(simplev2f IN) : SV_Target
			{
				fixed4 color = IN.color;
				float4 sdf = tex2D(_MainTex, IN.texcoord);
				float4 sdfShadow = tex2D(_MainTex, IN.texcoord1);

				//setup antialiazing
				float dx = ddx(IN.texcoord.x) * _MainTex_TexelSize.z;
				float dy = ddy(IN.texcoord.y) * _MainTex_TexelSize.w;

				//_SDFDistance is the distance of the field. Could be controlled by blue channel.
				float factor = _SDFDistance *(1-_Softness);
				float toPixels = max(1,factor * rsqrt(dx*dx+dy*dy));


				sdf = saturate((sdf-0.5) *toPixels +0.5);
				color.a = saturate(sdf.r + sdf.g + sdf.b);

				fixed shadowBlur = (1-_ShadowDistance)*10 +1;
				sdfShadow;
				sdfShadow = saturate((sdfShadow-0.5) * shadowBlur+0.5);
				sdfShadow.a = saturate(sdfShadow.r + color.a);
				sdfShadow.a *= sdfShadow.a;
				sdfShadow.rgb = color.rgb * color.a;

				color = sdfShadow;

				color.rgb *= color.a;

				return color;
			}

		ENDCG
		}
	}
}

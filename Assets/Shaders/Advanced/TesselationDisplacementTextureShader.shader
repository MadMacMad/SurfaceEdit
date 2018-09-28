Shader "SurfaceEdit/Advanced/TesselationDisplacementTextureShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Displacement("Displacement", 2D) = "gray" {}
		_DisplacementIntensity("Displacement Intensity", Range(0, 1)) = 1
		_TesselationMultiplier("Tesselation Multiplier", Range(1, 64)) = 3
		[Toggle] _IsNormal("Is Normal", Float) = 0
		[Toggle] _InvertNormal("Invert Normal", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma tessellate tess  
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 5.0

			#include "Tessellation.cginc"

			sampler2D _MainTex;
			sampler2D _Displacement;

			float _DisplacementIntensity;
			float _TesselationMultiplier;

			float _InvertNormal;
			float _IsNormal;

			float4 tess()
			{
				return _TesselationMultiplier;
			}

			struct appdata_t
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;

				float rawDisp = tex2Dlod(_Displacement, float4(v.texcoord.xy, 0, 0)).r - .6;
				float d = rawDisp * _DisplacementIntensity;

				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * d);
				o.texcoord = v.texcoord;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 value = tex2D(_MainTex, i.texcoord);
				if (_IsNormal == 1)
					if (_InvertNormal == 1)
						value.g = 1 - value.g;
				return value;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}

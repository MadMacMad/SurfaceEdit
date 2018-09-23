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

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows vertex:vert tessellate:tess nolightmap addshadow 

		#pragma target 5.0

		#include "Tessellation.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _Displacement;

		float _DisplacementIntensity;
		float _TesselationMultiplier;

		float _InvertNormal;
		float _IsNormal;

		void vert(inout appdata v)
		{
			float rawDisp = tex2Dlod(_Displacement, float4(v.texcoord.xy, 0, 0)).r - 0.5;
			float d = rawDisp * _DisplacementIntensity;
			v.vertex.xyz += v.normal * d;
		}

		float4 tess()
		{
			return _TesselationMultiplier;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 value = tex2D(_MainTex, IN.uv_MainTex);
			if (_IsNormal == 1)
				if (_InvertNormal == 1)
					value.g = 1 - value.g;
			o.Albedo = value.rgb * value.a + float4(1, 0, 1, 1) * (1 - value.a);
			o.Specular = 0;
			o.Gloss = 0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

Shader "Custom/TesselationDisplacementShader" {
	Properties {
		_MainTex("Albedo", 2D) = "white" {}
		[Normal] _Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "black" {}
		_Displacement("Displacement", 2D) = "gray" {}
		_DisplacementIntensity("Displacement Intensity", Range(0, 1)) = 1
		_TesselationMultiplier("Tesselation Multiplier", Range(1, 64)) = 3
		[Toggle] _InvertNormal("Invert Normal", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tess nolightmap addshadow 

		#pragma target 5.0

		#include "Tessellation.cginc"

		struct appdata {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Metallic;
		sampler2D _Roughness;
		sampler2D _Displacement;

		float _DisplacementIntensity;
		float _TesselationMultiplier;

		float _InvertNormal;

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

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Metallic = tex2D(_Metallic, IN.uv_MainTex);
			o.Smoothness = 1 - tex2D(_Roughness, IN.uv_MainTex);
			float4 normal = tex2D(_Normal, IN.uv_MainTex);
			if (_InvertNormal == 1)
				normal.g = 1 - normal.g;
			o.Normal = UnpackNormal(normal);
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

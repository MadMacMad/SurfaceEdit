Shader "Tilify/Procedural/Brush"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" { }
		_QuadScaleX("Quad Scale X", Range (.00001, 10)) = .1
		_QuadScaleY("Quad Scale Y", Range (.00001, 10)) = .1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha, One One
			CGPROGRAM

			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#include "UnityCG.cginc"

			struct VertexData
			{
				float4 position : POSITION;
			};

			struct VertexOutput
			{
				float4 position : SV_POSITION;
			};
			struct GeometryOutput
			{
				float4 position : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _QuadScaleX;
			float _QuadScaleY;

			VertexOutput vert(VertexData v)
			{
				VertexOutput o;
				o.position = v.position - float4(_QuadScaleX * .5, _QuadScaleY * .5, 0, 0);
				return o;
			}
			
			[maxvertexcount(6)]
			void geom (point VertexOutput input[1], inout TriangleStream<GeometryOutput> OutputStream)
			{
				float4 position = input[0].position;
				GeometryOutput output;

				output.position = UnityObjectToClipPos(position);
				output.texcoord = float2(0, 0);
				OutputStream.Append(output);

				output.position = UnityObjectToClipPos(position + float4(0, _QuadScaleY, 0, 0));
				output.texcoord = float2(0, 1);
				OutputStream.Append(output);

				output.position = UnityObjectToClipPos(position + float4(_QuadScaleX, 0, 0, 0));
				output.texcoord = float2(1, 0);
				OutputStream.Append(output);

				OutputStream.RestartStrip();

				output.position = UnityObjectToClipPos(position + float4(0, _QuadScaleY, 0, 0));
				output.texcoord = float2(0, 1);
				OutputStream.Append(output);

				output.position = UnityObjectToClipPos(position + float4(_QuadScaleX, _QuadScaleY, 0, 0));
				output.texcoord = float2(1, 1);
				OutputStream.Append(output);

				output.position = UnityObjectToClipPos(position + float4(_QuadScaleX, 0, 0, 0));
				output.texcoord = float2(1, 0);
				OutputStream.Append(output);
			}
			float4 frag(GeometryOutput i) : SV_Target
			{
				return tex2D(_MainTex, i.texcoord);
			}

			ENDCG
		}
	}
}

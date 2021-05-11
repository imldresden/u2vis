Shader "Custom/ScatterplotShader"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_Size("Size", Range(0, 1)) = 0.5
		_Color("Main Color", Color) = (1,1,1,1)
	}

		SubShader
		{
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				Name "OnscreenGeometry"
				Tags { 
					"Queue" = "3000" 
					"IgnoreProjector" = "True" 
					"RenderType" = "Transparent" 
				}
				Zwrite On
				ZTest On
				//ZTest LEqual
				//Blend[_MySrcMode][_MyDstMode]
				Cull Off
				//Lighting Off
				//Offset -1, -1 // This line is added to default Unlit/Transparent shader
				LOD 200

				CGPROGRAM
					#pragma target 5.0
					#pragma multi_compile_instancing
					#pragma vertex VS_Main
					#pragma fragment FS_Main alpha
					#pragma geometry GS_Main
					#include "UnityCG.cginc"
					


			// **************************************************************
			// Data structures												*
			// **************************************************************

			struct VS_INPUT {
				float4 position : POSITION;
				float4 color: COLOR;
				float3 normal: NORMAL;
				float4 _MainTex : TEXCOORD0; // index, vertex size, filtered, prev size
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct GS_INPUT
			{
				float4	pos : SV_POSITION;
				float2  tex0 : TEXCOORD0;
				float4  color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct FS_INPUT
			{
				float4	pos : POSITION;
				float2  tex0 : TEXCOORD0;
				float4  color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct FS_OUTPUT
			{
				float4 color : COLOR;
				float depth : SV_Depth;
			};

			// **************************************************************
			// Vars															*
			// **************************************************************

			float _Size;
			float4 _Color;
			sampler2D _MainTex;


			// **************************************************************
			// Shader Programs												*
			// **************************************************************

			// Vertex Shader ------------------------------------------------
			GS_INPUT VS_Main(VS_INPUT v)
			{
				GS_INPUT output;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(GS_INPUT, output);					
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				//output.pos = UnityObjectToClipPos(v.position);
				output.pos = v.position;
				output.tex0 = v._MainTex;
				output.color = v.color;

				return output;
			}



			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(8)]
			void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
			{
				UNITY_SETUP_INSTANCE_ID(p[0]);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(p[0]);

				float3 up = UNITY_MATRIX_IT_MV[1].xyz;
				float3 right = -UNITY_MATRIX_IT_MV[0].xyz;


				float halfS = 0.1f * _Size;

				float4 v[4];

				v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
				v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
				v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
				v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

				FS_INPUT pIn;

				pIn.color = p[0].color;

				pIn.pos = UnityObjectToClipPos(v[0]);
				pIn.tex0 = float2(1.0f, 0.0f);
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
				triStream.Append(pIn);

				pIn.pos = UnityObjectToClipPos(v[1]);
				pIn.tex0 = float2(1.0f, 1.0f);
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
				triStream.Append(pIn);

				pIn.pos = UnityObjectToClipPos(v[2]);
				pIn.tex0 = float2(0.0f, 0.0f);
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
				triStream.Append(pIn);

				pIn.pos = UnityObjectToClipPos(v[3]);
				pIn.tex0 = float2(0.0f, 1.0f);
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
				triStream.Append(pIn);

			}

			// Fragment Shader -----------------------------------------------
			FS_OUTPUT FS_Main(FS_INPUT input)
			{
				FS_OUTPUT o;
				o.color = tex2D(_MainTex, input.tex0.xy) * input.color * _Color;
				//o.color = _Color;
				o.depth = o.color.a > 0.5 ? input.pos.z : 0;
				return o;
			}

			ENDCG
		} // end pass
	}
}

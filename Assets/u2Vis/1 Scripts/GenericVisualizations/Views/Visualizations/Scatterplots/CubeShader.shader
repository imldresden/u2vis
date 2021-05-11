// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CubeShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Size("Size", Range(0, 1)) = 0.5
		_Color("Main Color", Color) = (1,1,1,1)
	}

		SubShader
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Pass
			{
				Name "Onscreen geometry"
				Tags 			{
			"LightMode" = "ForwardBase"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			}
			
			//Blend[_MySrcMode][_MyDstMode]
			//Blend One One
			ColorMaterial AmbientAndDiffuse
			Lighting Off
			ZWrite On
			ZTest[unity_GUIZTestMode]
			Cull Off

				CGPROGRAM
					#pragma target 5.0
					#pragma vertex VS_Main
					#pragma fragment FS_Main
					#pragma geometry GS_Main
					#include "UnityCG.cginc"
					#include "UnityLightingCommon.cginc"


			// **************************************************************
			// Data structures												*
			// **************************************************************

			struct VS_INPUT {
				float4 position : POSITION;
				float4 color: COLOR;
				float3 normal: NORMAL;
				float4 _MainTex : TEXCOORD0; // index, vertex size, filtered, prev size
			};

			struct GS_INPUT
			{
				float4	vertex : SV_POSITION;
				float2  tex0 : TEXCOORD0;
				float4  color : COLOR;
				float3	normal : NORMAL;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 tex0	: TEXCOORD0;
			};

			struct FS_OUTPUT
			{
				float4 color : COLOR;
				//float depth : SV_Depth;
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

			void emitCube(float3 position, float4 color, float size, inout TriangleStream<g2f> triStream)
			{
				float xsize = size / (unity_ObjectToWorld[0].x / unity_ObjectToWorld[2].z);
				float ysize = size / (unity_ObjectToWorld[1].y / unity_ObjectToWorld[2].z);
				float zsize = size / (unity_ObjectToWorld[2].z / unity_ObjectToWorld[1].y);


				float3 NEU = float3(xsize, ysize, zsize);
				float3 NED = float3(xsize, -ysize, zsize);
				float3 NWU = float3(-xsize, ysize, zsize);
				float3 NWD = float3(-xsize, -ysize, zsize);
				float3 SEU = float3(xsize, ysize, -zsize);
				float3 SED = float3(xsize, -ysize, -zsize);
				float3 SWU = float3(-xsize, ysize, -zsize);
				float3 SWD = float3(-xsize, -ysize, -zsize);

				float4 pNEU = float4(position + NEU, 1.0f);
				float4 pNED = float4(position + NED, 1.0f);
				float4 pNWU = float4(position + NWU, 1.0f);
				float4 pNWD = float4(position + NWD, 1.0f);

				float4 pSEU = float4(position + SEU, 1.0f);
				float4 pSED = float4(position + SED, 1.0f);
				float4 pSWU = float4(position + SWU, 1.0f);
				float4 pSWD = float4(position + SWD, 1.0f);

				float3 nN = float3(0, 0, 1);
				float3 nS = float3(0, 0, -1);
				float3 nE = float3(-1, 0, 0);
				float3 nW = float3(1, 0, 0);
				float3 nU = float3(0, 1, 0);
				float3 nD = float3(1, -1, 0);

				float4x4 vp = UNITY_MATRIX_MVP;

				g2f pIn;

				// FACE 1

				pIn.color = color;

				// FACE 1
				half nl;
				half3 worldNormal;

				worldNormal = UnityObjectToWorldNormal(nN);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;

				pIn.vertex = UnityObjectToClipPos(pNWU);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNEU);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNWD);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNED);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();

				// FACE 2

				worldNormal = UnityObjectToWorldNormal(nW);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;


				pIn.vertex = UnityObjectToClipPos(pNED);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNEU);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSED);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSEU);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();

				// FACE 3

				worldNormal = UnityObjectToWorldNormal(nU);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;



				pIn.vertex = UnityObjectToClipPos(pNWU);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNEU);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSWU);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSEU);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();

				// FACE 4
				worldNormal = UnityObjectToWorldNormal(nS);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;


				pIn.vertex = UnityObjectToClipPos(pSWU);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSEU);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSWD);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSED);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();

				// FACE 5
				worldNormal = UnityObjectToWorldNormal(nD);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;


				pIn.vertex = UnityObjectToClipPos(pNWD);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNED);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSWD);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSED);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();

				// FACE 6
				worldNormal = UnityObjectToWorldNormal(nE);
				nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				pIn.color = float4(_LightColor0.rgb * nl, color.a);
				pIn.color.rgb += ShadeSH9(half4(worldNormal, 1));
				pIn.color.rgb *= color.rgb;

				pIn.vertex = UnityObjectToClipPos(pNWD);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pNWU);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSWD);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.vertex = UnityObjectToClipPos(pSWU);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);

				triStream.RestartStrip();
			}

			// Vertex Shader ------------------------------------------------
			GS_INPUT VS_Main(VS_INPUT v)
			{
				GS_INPUT output;

				//output.pos = UnityObjectToClipPos(v.position);
				output.vertex = v.position;
				output.tex0 = v._MainTex;
				output.color = v.color;

				return output;
			}



			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(100)]
			void GS_Main(point GS_INPUT p[1], inout TriangleStream<g2f> triStream)
			{

				float halfS = 0.1f * _Size;

				emitCube(p[0].vertex, p[0].color, halfS, triStream);
			}

			// Fragment Shader -----------------------------------------------
			FS_OUTPUT FS_Main(g2f input)
			{
				FS_OUTPUT o;
				o.color = tex2D(_MainTex, input.tex0.xy)*_Color*input.color;
				//o.color = _Color;
				//o.depth = o.color.a > 0.5 ? input.pos.z : 0;
				return o;
			}

			ENDCG
		} // end pass
		}
}
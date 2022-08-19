// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/SoftParticle"
{
	Properties
	{
		_Color("Color", Color) = (0.4764151,0.9408201,1,0)
		_Diffuse("Diffuse", 2D) = "white" {}
		_SoftParticleFactor("Soft Particle Factor", Range( 0 , 10)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			half2 uv_texcoord;
			half4 screenPosition80;
		};

		uniform half4 _Color;
		uniform sampler2D _Diffuse;
		uniform half4 _Diffuse_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _SoftParticleFactor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 vertexPos80 = ase_vertex3Pos;
			float4 ase_screenPos80 = ComputeScreenPos( UnityObjectToClipPos( vertexPos80 ) );
			o.screenPosition80 = ase_screenPos80;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			half4 tex2DNode9 = tex2D( _Diffuse, uv_Diffuse );
			o.Emission = ( _Color * tex2DNode9 ).rgb;
			float4 ase_screenPos80 = i.screenPosition80;
			float4 ase_screenPosNorm80 = ase_screenPos80 / ase_screenPos80.w;
			ase_screenPosNorm80.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm80.z : ase_screenPosNorm80.z * 0.5 + 0.5;
			float screenDepth80 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos80 )));
			float distanceDepth80 = abs( ( screenDepth80 - LinearEyeDepth( ase_screenPosNorm80.z ) ) / ( _SoftParticleFactor ) );
			o.Alpha = ( tex2DNode9.a * saturate( distanceDepth80 ) * _Color.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
196;133;1228;878;918.9317;377.6217;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;81;-951.8075,279.1663;Float;False;Property;_SoftParticleFactor;Soft Particle Factor;3;0;Create;True;0;0;False;0;0.5;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;82;-868.3976,103.8649;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;80;-605.8818,182.7348;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-455.9091,-51.02061;Float;True;Property;_Diffuse;Diffuse;2;0;Create;True;0;0;False;0;None;bb4b4370dcb8aa24a9ecb8aee5ce9648;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-386.909,-237.422;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;0.4764151,0.9408201,1,0;0.3025098,0.5723806,0.7450981,0.854902;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;85;-309.356,188.3893;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-66.4999,-107.2;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-65.05418,159.1748;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;84;121.6728,-108.3395;Half;False;True;2;Half;ASEMaterialInspector;0;0;Unlit;IL3DN/SoftParticle;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;80;1;82;0
WireConnection;80;0;81;0
WireConnection;85;0;80;0
WireConnection;10;0;2;0
WireConnection;10;1;9;0
WireConnection;83;0;9;4
WireConnection;83;1;85;0
WireConnection;83;2;2;4
WireConnection;84;2;10;0
WireConnection;84;9;83;0
ASEEND*/
//CHKSM=870E819BE56E7D8795E6AA064C1C5799DA12C915
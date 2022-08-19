// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/Grass"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_SNOW_ON)] _Snow("Snow", Float) = 1
		[Toggle(_WIND_ON)] _Wind("Wind", Float) = 1
		_WindStrenght("Wind Strenght", Range( 0 , 1)) = 0.5
		[Toggle(_WIGGLE_ON)] _Wiggle("Wiggle", Float) = 1
		_WiggleStrenght("Wiggle Strenght", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma multi_compile __ _WIND_ON
		#pragma multi_compile __ _SNOW_ON
		#pragma multi_compile __ _WIGGLE_ON
		#pragma exclude_renderers vulkan xbox360 psp2 n3ds wiiu 
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows nolightmap  nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float3 WindDirection;
		uniform sampler2D NoiseTextureFloat;
		uniform float WindSpeedFloat;
		uniform float WindTurbulenceFloat;
		uniform float _WindStrenght;
		uniform float WindStrenghtFloat;
		uniform float SnowGrassFloat;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float GrassWiggleFloat;
		uniform float _WiggleStrenght;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_output_1056_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner1060 = ( 1.0 * _Time.y * ( temp_output_1056_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xz);
			float4 worldNoise1038 = ( tex2Dlod( NoiseTextureFloat, float4( ( ( panner1060 * WindTurbulenceFloat ) / float2( 10,10 ) ), 0, 0.0) ) * _WindStrenght * WindStrenghtFloat );
			float4 transform1029 = mul(unity_WorldToObject,( float4( WindDirection , 0.0 ) * ( ( v.color.a * worldNoise1038 ) + ( worldNoise1038 * v.color.g ) ) ));
			#ifdef _WIND_ON
				float4 staticSwitch1031 = transform1029;
			#else
				float4 staticSwitch1031 = float4( 0,0,0,0 );
			#endif
			v.vertex.xyz += staticSwitch1031.xyz;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			#ifdef _SNOW_ON
				float staticSwitch1049 = ( saturate( pow( ( 1.0 - i.vertexColor.g ) , 2.0 ) ) * SnowGrassFloat );
			#else
				float staticSwitch1049 = 0.0;
			#endif
			float3 temp_output_1056_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = i.worldPos;
			float2 panner1060 = ( 1.0 * _Time.y * ( temp_output_1056_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xz);
			float4 worldNoise1038 = ( tex2D( NoiseTextureFloat, ( ( panner1060 * WindTurbulenceFloat ) / float2( 10,10 ) ) ) * _WindStrenght * WindStrenghtFloat );
			float cos1075 = cos( ( ( tex2D( NoiseTextureFloat, worldNoise1038.rg ) * i.vertexColor.g ) * GrassWiggleFloat * _WiggleStrenght ).r );
			float sin1075 = sin( ( ( tex2D( NoiseTextureFloat, worldNoise1038.rg ) * i.vertexColor.g ) * GrassWiggleFloat * _WiggleStrenght ).r );
			float2 rotator1075 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos1075 , -sin1075 , sin1075 , cos1075 )) + float2( 0.5,0.5 );
			#ifdef _WIGGLE_ON
				float2 staticSwitch1033 = rotator1075;
			#else
				float2 staticSwitch1033 = i.uv_texcoord;
			#endif
			float4 tex2DNode97 = tex2D( _MainTex, staticSwitch1033 );
			o.Albedo = saturate( ( staticSwitch1049 + ( _Color * tex2DNode97 ) ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode97.a - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=17009
667;349;1924;1020;-407.37;-716.1224;1.6;True;False
Node;AmplifyShaderEditor.CommentaryNode;1052;1265.961,1465.902;Inherit;False;1606.407;663.0706;World Noise;15;1065;1064;1063;1062;1061;1059;1060;1057;1058;1054;1078;1055;1056;1053;1077;World Noise;1,0,0.02020931,1;0;0
Node;AmplifyShaderEditor.Vector3Node;867;961.4112,1361.905;Float;False;Global;WindDirection;WindDirection;14;0;Create;True;0;0;False;0;0,0,0;-0.7071068,0,-0.7071068;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;1053;1295.597,1741.238;Inherit;False;FLOAT2;0;2;1;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1055;1295.207,1525.581;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;1056;1503.598,1741.238;Inherit;False;World;World;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1078;1581.201,2015.507;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1054;1460.903,1916.038;Float;False;Global;WindSpeedFloat;WindSpeedFloat;3;0;Create;False;0;0;False;0;0.5;0.281;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;1057;1579.412,1521.734;Inherit;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;1768.826,1899.9;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;1060;1931.242,1532.904;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1059;1839.375,1677.775;Float;False;Global;WindTurbulenceFloat;WindTurbulenceFloat;4;0;Create;False;0;0;False;0;0.5;0.121;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1061;2147.692,1529.906;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1077;2302.947,1528.563;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;10,10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1064;2357.26,1902.282;Float;False;Global;WindStrenghtFloat;WindStrenghtFloat;3;0;Create;False;0;0;False;0;0.5;0.834;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1063;2360.906,1796.001;Float;False;Property;_WindStrenght;Wind Strenght;6;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1062;2443.763,1532.144;Inherit;True;Global;NoiseTextureFloat;NoiseTextureFloat;4;0;Create;False;0;0;False;0;None;e5055e0d246bd1047bdb28057a93753c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1065;2734.054,1777.316;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1038;2936.422,1769.995;Float;False;worldNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1040;1669.801,810.623;Inherit;False;1038;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1066;1876.59,737.8658;Inherit;False;1007.189;586.5881;UV Animation;8;1075;1074;1073;1072;1071;1070;1068;1076;UV Animation;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1068;1985.426,785.037;Inherit;True;Property;_TextureSample1;Texture Sample 0;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1062;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1076;2122.001,979.407;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;1042;1810.693,275.977;Inherit;False;1075.409;358.2535;Snow;6;1048;1047;1046;1045;1044;1043;Snow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1072;2025.163,1141.254;Float;False;Global;GrassWiggleFloat;GrassWiggleFloat;4;0;Create;False;0;0;False;0;0.5;0.526;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1071;2026.485,1230.682;Float;False;Property;_WiggleStrenght;Wiggle Strenght;8;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1070;2332.4,1011.992;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;1043;1937.631,332.4831;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1073;2478.469,1012.831;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1044;2164.793,373.7453;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1041;2261.57,2212.71;Inherit;False;610.9202;680.9117;Vertex Animation;5;857;854;855;853;856;Vertex Animation;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1074;2369.859,842.5597;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;856;2311.683,2711.625;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;1045;2356.103,375.3198;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;853;2316.312,2301.206;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;1075;2621.79,893.527;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;746;3306.542,776.9384;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1039;2023.244,2536.141;Inherit;False;1038;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1046;2535.954,382.5026;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;855;2554.373,2611.771;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;854;2549.611,2389.879;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1047;1841.962,518.8069;Inherit;False;Global;SnowGrassFloat;SnowGrassFloat;5;0;Create;False;0;0;False;0;1;0.6;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1033;3577.498,858.7823;Float;False;Property;_Wiggle;Wiggle;7;0;Create;True;0;0;False;0;1;1;1;True;_WIND_ON;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;97;3882.001,1127.666;Inherit;True;Property;_MainTex;MainTex;2;1;[NoScaleOffset];Create;True;0;0;False;0;None;a73c218e0d8156240a793d22710686d1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1048;2741.46,496.2007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;292;3969.53,927.8489;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;0.3764702,0.470588,0.188235,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;857;2734.372,2494.985;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;4335.922,1060.561;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;3171.533,1371.818;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;1049;3577.167,472.4447;Inherit;False;Property;_Snow;Snow;3;0;Create;True;0;0;False;0;1;1;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1050;4490.768,761.2556;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;1029;3342.364,1373.903;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;1031;3576.195,1345.762;Float;False;Property;_Wind;Wind;5;0;Create;True;0;0;False;0;1;1;1;True;_WIND_ON;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;1036;3908.093,1466.44;Float;False;Property;_AlphaCutoff;Alpha Cutoff;1;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1051;4665.86,1095.494;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4839.161,1102.959;Float;False;True;2;;0;0;Lambert;IL3DN/Grass;False;False;False;False;False;False;True;False;True;False;False;False;True;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;9;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;xboxone;ps4;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;1036;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1053;0;867;0
WireConnection;1056;0;1053;0
WireConnection;1057;0;1055;0
WireConnection;1058;0;1056;0
WireConnection;1058;1;1054;0
WireConnection;1058;2;1078;0
WireConnection;1060;0;1057;0
WireConnection;1060;2;1058;0
WireConnection;1061;0;1060;0
WireConnection;1061;1;1059;0
WireConnection;1077;0;1061;0
WireConnection;1062;1;1077;0
WireConnection;1065;0;1062;0
WireConnection;1065;1;1063;0
WireConnection;1065;2;1064;0
WireConnection;1038;0;1065;0
WireConnection;1068;1;1040;0
WireConnection;1070;0;1068;0
WireConnection;1070;1;1076;2
WireConnection;1073;0;1070;0
WireConnection;1073;1;1072;0
WireConnection;1073;2;1071;0
WireConnection;1044;0;1043;2
WireConnection;1045;0;1044;0
WireConnection;1075;0;1074;0
WireConnection;1075;2;1073;0
WireConnection;1046;0;1045;0
WireConnection;855;0;1039;0
WireConnection;855;1;856;2
WireConnection;854;0;853;4
WireConnection;854;1;1039;0
WireConnection;1033;1;746;0
WireConnection;1033;0;1075;0
WireConnection;97;1;1033;0
WireConnection;1048;0;1046;0
WireConnection;1048;1;1047;0
WireConnection;857;0;854;0
WireConnection;857;1;855;0
WireConnection;293;0;292;0
WireConnection;293;1;97;0
WireConnection;872;0;867;0
WireConnection;872;1;857;0
WireConnection;1049;0;1048;0
WireConnection;1050;0;1049;0
WireConnection;1050;1;293;0
WireConnection;1029;0;872;0
WireConnection;1031;0;1029;0
WireConnection;1051;0;1050;0
WireConnection;0;0;1051;0
WireConnection;0;10;97;4
WireConnection;0;11;1031;0
ASEEND*/
//CHKSM=DCC09191A85DB25DC09249205DAB5981DBADA050
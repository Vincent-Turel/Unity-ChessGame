// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/CardSimple"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_WIND_ON)] _Wind("Wind", Float) = 1
		_WindStrenght("Wind Strenght", Range( 0 , 1)) = 0.5
		[Toggle(_ISDEAD_ON)] _IsDead("Is Dead", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma multi_compile __ _WIND_ON
		#pragma shader_feature _ISDEAD_ON
		#pragma exclude_renderers vulkan xbox360 psp2 n3ds wiiu 
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows nolightmap  nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float3 WindDirection;
		uniform sampler2D NoiseTextureFloat;
		uniform float WindSpeedFloat;
		uniform float WindTurbulenceFloat;
		uniform float _WindStrenght;
		uniform float WindStrenghtFloat;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_output_913_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner918 = ( 1.0 * _Time.y * ( temp_output_913_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xy);
			float4 worldNoise905 = ( tex2Dlod( NoiseTextureFloat, float4( ( ( panner918 * WindTurbulenceFloat ) / float2( 10,10 ) ), 0, 0.0) ) * _WindStrenght * WindStrenghtFloat );
			float4 transform886 = mul(unity_WorldToObject,( float4( WindDirection , 0.0 ) * ( ( v.color.a * worldNoise905 ) + ( worldNoise905 * v.color.g ) ) ));
			#ifdef _WIND_ON
				float4 staticSwitch897 = transform886;
			#else
				float4 staticSwitch897 = float4( 0,0,0,0 );
			#endif
			v.vertex.xyz += staticSwitch897.xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			o.Alpha = 1;
			float2 uv_TexCoord901 = i.uv_texcoord + float2( 0,-0.5 );
			#ifdef _ISDEAD_ON
				float2 staticSwitch902 = uv_TexCoord901;
			#else
				float2 staticSwitch902 = i.uv_texcoord;
			#endif
			clip( tex2D( _MainTex, staticSwitch902 ).a - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17009
667;361;1924;1008;-402.9771;-108.3782;2.5;True;False
Node;AmplifyShaderEditor.CommentaryNode;910;1733.23,721.8276;Inherit;False;1617.335;623.9181;World Noise;15;923;921;922;920;919;917;918;915;916;925;912;913;914;911;924;World Noise;1,0,0.02020931,1;0;0
Node;AmplifyShaderEditor.Vector3Node;867;1187.758,1339.689;Float;False;Global;WindDirection;WindDirection;14;0;Create;True;0;0;False;0;0,0,0;-0.8335966,0,0.5523738;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;911;1762.867,997.1633;Inherit;False;FLOAT2;0;2;1;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformDirectionNode;913;1970.867,997.1633;Inherit;False;World;World;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;912;1928.173,1173.115;Float;False;Global;WindSpeedFloat;WindSpeedFloat;3;0;Create;False;0;0;False;0;0.5;0.447;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;914;1760.149,781.0645;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;925;2048.41,1270.901;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;916;2230.055,1146.55;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;915;2046.977,783.0656;Inherit;False;FLOAT2;0;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;918;2330.185,784.3235;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;917;2232.31,960.5212;Float;False;Global;WindTurbulenceFloat;WindTurbulenceFloat;4;0;Create;False;0;0;False;0;0.5;0.194;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;919;2591.209,783.6376;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;924;2764.415,786.0791;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;10,10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;922;2904.17,1051.927;Float;False;Property;_WindStrenght;Wind Strenght;5;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;921;2903.429,778.5692;Inherit;True;Global;NoiseTextureFloat;NoiseTextureFloat;3;0;Create;False;0;0;False;0;None;e5055e0d246bd1047bdb28057a93753c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;920;2904.323,1156.307;Float;False;Global;WindStrenghtFloat;WindStrenghtFloat;3;0;Create;False;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;923;3201.319,1033.242;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;905;3377.68,1029.7;Float;False;worldNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;907;2756.086,1485.109;Inherit;False;594.1707;668.2979;Vertex Animation;5;857;855;854;856;853;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;906;2520.913,1789.344;Inherit;False;905;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;853;2821.003,1548.887;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;856;2827.393,1953.275;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;854;3034.68,1659.4;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;855;3041.06,1892.946;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;909;3603.217,762.1166;Inherit;False;264.2837;355.7888;Offset UV;2;903;901;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;857;3212.787,1755.604;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;901;3630.875,970.2458;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;903;3632.374,823.1782;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;3568.628,1342.612;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;886;3753.615,1344.448;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;902;3946.612,883.7297;Float;False;Property;_IsDead;Is Dead;6;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;897;3982.062,1313.741;Float;False;Property;_Wind;Wind;4;0;Create;True;0;0;False;0;1;1;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;97;4172.32,1005.774;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;False;0;None;46fa255f11489784a86ff91e1485cca5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;292;4247.793,815.789;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;0.06421296,0.310983,0.3679241,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;904;4181.503,1534.129;Float;False;Property;_AlphaCutoff;Alpha Cutoff;1;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;900;4851.913,1065.111;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;IL3DN/CardSimple;False;False;False;False;False;False;True;False;True;False;False;False;False;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;9;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;xboxone;ps4;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;904;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;911;0;867;0
WireConnection;913;0;911;0
WireConnection;916;0;913;0
WireConnection;916;1;912;0
WireConnection;916;2;925;0
WireConnection;915;0;914;0
WireConnection;918;0;915;0
WireConnection;918;2;916;0
WireConnection;919;0;918;0
WireConnection;919;1;917;0
WireConnection;924;0;919;0
WireConnection;921;1;924;0
WireConnection;923;0;921;0
WireConnection;923;1;922;0
WireConnection;923;2;920;0
WireConnection;905;0;923;0
WireConnection;854;0;853;4
WireConnection;854;1;906;0
WireConnection;855;0;906;0
WireConnection;855;1;856;2
WireConnection;857;0;854;0
WireConnection;857;1;855;0
WireConnection;872;0;867;0
WireConnection;872;1;857;0
WireConnection;886;0;872;0
WireConnection;902;1;903;0
WireConnection;902;0;901;0
WireConnection;897;0;886;0
WireConnection;97;1;902;0
WireConnection;900;2;292;0
WireConnection;900;10;97;4
WireConnection;900;11;897;0
ASEEND*/
//CHKSM=0F80944653A235B56BB2522DE2FA50FBF191B9A4
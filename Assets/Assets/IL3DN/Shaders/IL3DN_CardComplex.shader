// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/CardComplex"
{
	Properties
	{
		_Bark("Bark", Color) = (0,0,0,0)
		_Leaves("Leaves", Color) = (1,1,1,1)
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
		#pragma multi_compile __ _WIND_ON
		#pragma shader_feature _ISDEAD_ON
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows dithercrossfade vertex:vertexDataFunc 
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
		uniform float4 _Bark;
		uniform float4 _Leaves;
		uniform sampler2D _MainTex;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_output_917_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner921 = ( 1.0 * _Time.y * ( temp_output_917_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xy);
			float4 worldNoise908 = ( tex2Dlod( NoiseTextureFloat, float4( ( ( panner921 * WindTurbulenceFloat ) / float2( 10,10 ) ), 0, 0.0) ) * _WindStrenght * WindStrenghtFloat );
			float4 transform886 = mul(unity_WorldToObject,( float4( WindDirection , 0.0 ) * ( ( v.color.a * worldNoise908 ) + ( worldNoise908 * v.color.g ) ) ));
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
			float2 uv_TexCoord905 = i.uv_texcoord + float2( 0,-0.5 );
			#ifdef _ISDEAD_ON
				float2 staticSwitch903 = uv_TexCoord905;
			#else
				float2 staticSwitch903 = i.uv_texcoord;
			#endif
			float4 tex2DNode97 = tex2D( _MainTex, staticSwitch903 );
			float4 lerpResult901 = lerp( _Bark , _Leaves , tex2DNode97.g);
			o.Emission = lerpResult901.rgb;
			o.Alpha = 1;
			clip( tex2DNode97.a - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17009
667;361;1924;1008;-1259.096;-150.7144;2.141272;True;False
Node;AmplifyShaderEditor.CommentaryNode;913;1704.758,734.1497;Inherit;False;1619.491;623.7284;World Noise;15;925;927;920;922;921;919;916;918;928;926;924;923;915;917;914;World Noise;1,0,0.02020931,1;0;0
Node;AmplifyShaderEditor.Vector3Node;867;1386.682,1334.824;Float;False;Global;WindDirection;WindDirection;14;0;Create;True;0;0;False;0;0,0,0;-0.8335966,0,0.5523738;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;914;1734.395,1009.486;Inherit;False;FLOAT2;0;2;1;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformDirectionNode;917;1942.394,1009.486;Inherit;False;World;World;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;915;1901,1185.436;Float;False;Global;WindSpeedFloat;WindSpeedFloat;3;0;Create;False;0;0;False;0;0.5;0.447;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;916;1741.375,789.4868;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;928;2012.95,1272.026;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;919;2018.804,787.088;Inherit;False;FLOAT2;0;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;918;2221.085,1127.871;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;921;2385.813,792.146;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;920;2290.838,969.5438;Float;False;Global;WindTurbulenceFloat;WindTurbulenceFloat;4;0;Create;False;0;0;False;0;0.5;0.194;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;922;2602.136,792.0601;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;927;2751.35,793.2261;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;10,10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;925;2890.557,793.9918;Inherit;True;Global;NoiseTextureFloat;NoiseTextureFloat;4;0;Create;False;0;0;False;0;None;e5055e0d246bd1047bdb28057a93753c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;924;2796.053,1170.528;Float;False;Global;WindStrenghtFloat;WindStrenghtFloat;3;0;Create;False;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;923;2799.699,1064.248;Float;False;Property;_WindStrenght;Wind Strenght;6;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;926;3172.847,1045.563;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;908;3367.546,1040.059;Float;False;worldNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;910;2760.928,1436.624;Inherit;False;556.1174;640.8626;Vertex Animation;5;857;855;854;853;856;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;909;2501.192,1727.107;Inherit;False;908;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;856;2802.693,1892.36;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;853;2800.965,1484.183;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;854;3013.426,1612.143;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;912;3599.307,792.2412;Inherit;False;264.2837;355.7888;Offset UV;2;905;906;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;855;3016.426,1805.143;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;857;3177.234,1697.599;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;905;3622.975,1001.848;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;906;3625.133,850.3633;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;3568.628,1342.612;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;903;3965.355,926.8776;Float;False;Property;_IsDead;Is Dead;7;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;292;4191.126,813.5336;Float;False;Property;_Leaves;Leaves;1;0;Create;True;0;0;False;0;1,1,1,1;0.06274467,0.3686269,0.2644943,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;902;4194.129,623.2159;Float;False;Property;_Bark;Bark;0;0;Create;True;0;0;False;0;0,0,0,0;0.5377356,0.3495415,0.1953092,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;97;4189.34,1094.845;Inherit;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;None;46fa255f11489784a86ff91e1485cca5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldToObjectTransfNode;886;3753.615,1344.448;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;897;3983.99,1315.858;Float;False;Property;_Wind;Wind;5;0;Create;True;0;0;False;0;1;1;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;901;4592.07,721.9071;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;907;4196.913,1436.703;Float;False;Property;_AlphaCutoff;Alpha Cutoff;2;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;900;4851.913,1065.111;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;IL3DN/CardComplex;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;907;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;914;0;867;0
WireConnection;917;0;914;0
WireConnection;919;0;916;0
WireConnection;918;0;917;0
WireConnection;918;1;915;0
WireConnection;918;2;928;0
WireConnection;921;0;919;0
WireConnection;921;2;918;0
WireConnection;922;0;921;0
WireConnection;922;1;920;0
WireConnection;927;0;922;0
WireConnection;925;1;927;0
WireConnection;926;0;925;0
WireConnection;926;1;923;0
WireConnection;926;2;924;0
WireConnection;908;0;926;0
WireConnection;854;0;853;4
WireConnection;854;1;909;0
WireConnection;855;0;909;0
WireConnection;855;1;856;2
WireConnection;857;0;854;0
WireConnection;857;1;855;0
WireConnection;872;0;867;0
WireConnection;872;1;857;0
WireConnection;903;1;906;0
WireConnection;903;0;905;0
WireConnection;97;1;903;0
WireConnection;886;0;872;0
WireConnection;897;0;886;0
WireConnection;901;0;902;0
WireConnection;901;1;292;0
WireConnection;901;2;97;2
WireConnection;900;2;901;0
WireConnection;900;10;97;4
WireConnection;900;11;897;0
ASEEND*/
//CHKSM=C12B7880C9A58E766B39A793918BC31ADB440B40
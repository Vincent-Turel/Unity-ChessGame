// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/Leaf"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
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
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma multi_compile __ _WIND_ON
		#pragma multi_compile __ _SNOW_ON
		#pragma multi_compile __ _WIGGLE_ON
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float3 WindDirection;
		uniform sampler2D NoiseTextureFloat;
		uniform float WindSpeedFloat;
		uniform float WindTurbulenceFloat;
		uniform float _WindStrenght;
		uniform float WindStrenghtFloat;
		uniform float SnowLeavesFloat;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float LeavesWiggleFloat;
		uniform float _WiggleStrenght;
		uniform float AlphaCutoffFloat;
		uniform float _AlphaCutoff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 temp_output_927_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 panner931 = ( 1.0 * _Time.y * ( temp_output_927_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xy);
			float4 worldNoise905 = ( tex2Dlod( NoiseTextureFloat, float4( ( ( panner931 * WindTurbulenceFloat ) / float2( 10,10 ) ), 0, 0.0) ) * _WindStrenght * WindStrenghtFloat );
			float4 transform886 = mul(unity_WorldToObject,( float4( WindDirection , 0.0 ) * ( ( v.color.a * worldNoise905 ) + ( worldNoise905 * v.color.g ) ) ));
			#ifdef _WIND_ON
				float4 staticSwitch897 = transform886;
			#else
				float4 staticSwitch897 = float4( 0,0,0,0 );
			#endif
			v.vertex.xyz += staticSwitch897.xyz;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldNormal = i.worldNormal;
			#ifdef _SNOW_ON
				float staticSwitch917 = ( saturate( pow( ase_worldNormal.y , 1.0 ) ) * SnowLeavesFloat );
			#else
				float staticSwitch917 = 0.0;
			#endif
			float3 temp_output_927_0 = float3( (WindDirection).xz ,  0.0 );
			float3 ase_worldPos = i.worldPos;
			float2 panner931 = ( 1.0 * _Time.y * ( temp_output_927_0 * WindSpeedFloat * 10.0 ).xy + (ase_worldPos).xy);
			float4 worldNoise905 = ( tex2D( NoiseTextureFloat, ( ( panner931 * WindTurbulenceFloat ) / float2( 10,10 ) ) ) * _WindStrenght * WindStrenghtFloat );
			float cos945 = cos( ( tex2D( NoiseTextureFloat, worldNoise905.rg ) * LeavesWiggleFloat * _WiggleStrenght ).r );
			float sin945 = sin( ( tex2D( NoiseTextureFloat, worldNoise905.rg ) * LeavesWiggleFloat * _WiggleStrenght ).r );
			float2 rotator945 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos945 , -sin945 , sin945 , cos945 )) + float2( 0.5,0.5 );
			#ifdef _WIGGLE_ON
				float2 staticSwitch898 = rotator945;
			#else
				float2 staticSwitch898 = i.uv_texcoord;
			#endif
			float4 tex2DNode97 = tex2D( _MainTex, staticSwitch898 );
			o.Albedo = saturate( ( staticSwitch917 + ( _Color * tex2DNode97 ) ) ).rgb;
			o.Alpha = 1;
			#ifdef _SNOW_ON
				float staticSwitch921 = AlphaCutoffFloat;
			#else
				float staticSwitch921 = 1.0;
			#endif
			clip( ( tex2DNode97.a / staticSwitch921 ) - _AlphaCutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers vulkan xbox360 psp2 n3ds wiiu 
		#pragma surface surf Lambert keepalpha fullforwardshadows nolightmap  nodirlightmap dithercrossfade vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=17009
667;349;1924;1020;-504.5933;-575.0248;1.803314;True;False
Node;AmplifyShaderEditor.Vector3Node;867;817.415,1344.312;Float;False;Global;WindDirection;WindDirection;14;0;Create;True;0;0;False;0;0,0,0;-0.7071068,0,-0.7071068;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;924;1183.945,1429.519;Inherit;False;1616.924;639.8218;World Noise;15;950;930;928;937;934;935;936;933;931;932;929;927;926;925;946;World Noise;1,0,0.02020931,1;0;0
Node;AmplifyShaderEditor.SwizzleNode;925;1213.581,1704.855;Inherit;False;FLOAT2;0;2;1;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;950;1493.9,1973.622;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;927;1421.582,1704.855;Inherit;False;World;World;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;928;1210.048,1483.933;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;926;1380.187,1880.807;Float;False;Global;WindSpeedFloat;WindSpeedFloat;3;0;Create;False;0;0;False;0;0.5;0.281;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;929;1505.27,1484.348;Inherit;False;FLOAT2;0;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;930;1671.314,1860.966;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;931;1835.208,1492.412;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;932;1747.459,1662.68;Float;False;Global;WindTurbulenceFloat;WindTurbulenceFloat;4;0;Create;False;0;0;False;0;0.5;0.121;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;933;2038.291,1492.52;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;946;2203.88,1489.946;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;10,10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;934;2275.242,1865.899;Float;False;Global;WindStrenghtFloat;WindStrenghtFloat;3;0;Create;False;0;0;False;0;0.5;0.834;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;935;2278.888,1759.618;Float;False;Property;_WindStrenght;Wind Strenght;7;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;936;2349.719,1485.238;Inherit;True;Global;NoiseTextureFloat;NoiseTextureFloat;5;0;Create;False;0;0;False;0;None;e5055e0d246bd1047bdb28057a93753c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;937;2652.036,1740.933;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;905;2872.686,1734.798;Float;False;worldNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;907;1562.809,830.8305;Inherit;False;905;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;938;1781.895,748.6516;Inherit;False;1012.714;535.89;UV Animation;6;945;944;943;942;941;940;UV Animation;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;940;1893.621,1079.119;Float;False;Global;LeavesWiggleFloat;LeavesWiggleFloat;5;0;Create;False;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;941;1891.647,1193.185;Float;False;Property;_WiggleStrenght;Wiggle Strenght;9;0;Create;False;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;942;1860.386,805.7994;Inherit;True;Property;_TextureSample1;Texture Sample 0;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;936;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;911;1727.516,274.4578;Inherit;False;1075.409;358.2535;Snow;5;916;915;914;913;912;Snow;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;944;2288.745,817.4724;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;943;2365.907,1099.493;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;908;2199.436,2102.639;Inherit;False;608.7889;673.9627;Vertex Animation;5;857;854;855;856;853;Vertex Animation;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;912;1763.288,348.2159;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RotatorNode;945;2533.803,951.7216;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;856;2257.083,2590.475;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;913;2039.218,394.2268;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;746;3442.74,1028.462;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;906;1936.904,2418.457;Inherit;False;905;worldNoise;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;853;2252.385,2177.409;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;914;2225.051,394.0062;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;898;3693.987,1190.03;Float;False;Property;_Wiggle;Wiggle;8;0;Create;True;0;0;False;0;1;1;0;True;_WIND_ON;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;915;1750.874,543.1254;Inherit;False;Global;SnowLeavesFloat;SnowLeavesFloat;4;0;Create;True;0;0;False;0;1;0.35;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;854;2522.822,2336.65;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;855;2525.823,2474.211;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;916;2451.704,524.162;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;857;2674.263,2397.85;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;97;3955.139,1167.211;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;False;0;None;6ab0f5f5ed2482e43a5ace7eeced19e6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;292;4011.016,958.6533;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,1;0.7058823,0.5882353,0.1843136,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;920;3644.657,1669.175;Inherit;False;Global;AlphaCutoffFloat;AlphaCutoffFloat;2;0;Create;False;0;0;False;0;2.1;2.1;1;2.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;917;3684.1,518.5128;Inherit;False;Property;_Snow;Snow;3;0;Create;True;0;0;False;0;1;1;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;919;3770.889,1559.604;Inherit;False;Constant;_Float1;Float 0;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;4285.25,1058.456;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;3059.303,1341.415;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;886;3253.275,1340.256;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;921;4156.484,1631.802;Inherit;False;Property;_Snow2;Snow;4;0;Create;True;0;0;False;0;1;1;0;True;_SNOW_ON;Toggle;2;Key0;Key1;Reference;917;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;918;4383.962,637.7845;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;923;4546.297,1048.91;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;910;3971.231,1403.038;Float;False;Property;_AlphaCutoff;Alpha Cutoff;1;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;897;3701.915,1311.666;Float;False;Property;_Wind;Wind;6;0;Create;True;0;0;False;0;1;1;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;922;4329.29,1262.201;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4713.688,1052.534;Float;False;True;2;;0;0;Lambert;IL3DN/Leaf;False;False;False;False;False;False;True;False;True;False;False;False;True;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;9;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;xboxone;ps4;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;892;-1;0;True;910;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;925;0;867;0
WireConnection;927;0;925;0
WireConnection;929;0;928;0
WireConnection;930;0;927;0
WireConnection;930;1;926;0
WireConnection;930;2;950;0
WireConnection;931;0;929;0
WireConnection;931;2;930;0
WireConnection;933;0;931;0
WireConnection;933;1;932;0
WireConnection;946;0;933;0
WireConnection;936;1;946;0
WireConnection;937;0;936;0
WireConnection;937;1;935;0
WireConnection;937;2;934;0
WireConnection;905;0;937;0
WireConnection;942;1;907;0
WireConnection;943;0;942;0
WireConnection;943;1;940;0
WireConnection;943;2;941;0
WireConnection;945;0;944;0
WireConnection;945;2;943;0
WireConnection;913;0;912;2
WireConnection;914;0;913;0
WireConnection;898;1;746;0
WireConnection;898;0;945;0
WireConnection;854;0;853;4
WireConnection;854;1;906;0
WireConnection;855;0;906;0
WireConnection;855;1;856;2
WireConnection;916;0;914;0
WireConnection;916;1;915;0
WireConnection;857;0;854;0
WireConnection;857;1;855;0
WireConnection;97;1;898;0
WireConnection;917;0;916;0
WireConnection;293;0;292;0
WireConnection;293;1;97;0
WireConnection;872;0;867;0
WireConnection;872;1;857;0
WireConnection;886;0;872;0
WireConnection;921;1;919;0
WireConnection;921;0;920;0
WireConnection;918;0;917;0
WireConnection;918;1;293;0
WireConnection;923;0;918;0
WireConnection;897;0;886;0
WireConnection;922;0;97;4
WireConnection;922;1;921;0
WireConnection;0;0;923;0
WireConnection;0;10;922;0
WireConnection;0;11;897;0
ASEEND*/
//CHKSM=E7FD6DD7867627B162EA161B0AB1544AEC52EF25
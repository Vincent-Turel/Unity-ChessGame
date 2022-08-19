// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/Water"
{
	Properties
	{
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalStrenght("Normal Strenght", Range( 0 , 1)) = 0.5
		_Size("Size", Range( 0 , 2)) = 0
		_Speed("Speed", Range( 0 , 1)) = 0
		_Transparency("Transparency", Range( 0 , 10)) = 0
		_Smoothness("Smoothness", Range( 0 , 10)) = 1
		_FoamColor("Foam Color", Color) = (0,0,0,0)
		_TopColor("Top Color", Color) = (0,0,0,0)
		_MidColor("Mid Color", Color) = (1,1,1,0)
		_BottomColor("Bottom Color", Color) = (0,0,0,0)
		_WaterFalloff("Water Falloff", Range( -1 , 0)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma exclude_renderers vulkan xbox360 psp2 n3ds wiiu 
		#pragma surface surf Standard alpha:fade keepalpha nolightmap  nodirlightmap 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float4 screenPos;
			half3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _NormalStrenght;
		uniform sampler2D _Normal;
		uniform float _Size;
		uniform float _Speed;
		uniform float4 _FoamColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _TopColor;
		uniform float4 _BottomColor;
		uniform float4 _MidColor;
		uniform float _WaterFalloff;
		uniform float _Smoothness;
		uniform float _Transparency;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			half2 temp_output_347_0 = ( float2( 100,100 ) * _Size );
			float mulTime242 = _Time.y * _Speed;
			half2 temp_cast_0 = (mulTime242).xx;
			float2 uv_TexCoord241 = i.uv_texcoord * temp_output_347_0 + temp_cast_0;
			half2 temp_cast_1 = (( 1.0 - mulTime242 )).xx;
			float2 uv_TexCoord364 = i.uv_texcoord * temp_output_347_0 + temp_cast_1;
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _Normal, uv_TexCoord241 ), _NormalStrenght ) , UnpackScaleNormal( tex2D( _Normal, uv_TexCoord364 ), _NormalStrenght ) );
			half3 ase_worldPos = i.worldPos;
			half2 temp_output_252_0 = (ase_worldPos).xz;
			half2 panner253 = ( 0.1 * _Time.y * float2( 1,0 ) + temp_output_252_0);
			half simplePerlin2D244 = snoise( ( panner253 * 1 ) );
			half2 panner292 = ( 0.1 * _Time.y * float2( -1,0 ) + temp_output_252_0);
			half simplePerlin2D290 = snoise( ( panner292 * 2 ) );
			half clampResult294 = clamp( ( simplePerlin2D244 + simplePerlin2D290 ) , 0.0 , 1.0 );
			float largeNoisePattern297 = clampResult294;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			half eyeDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float edgeEffect208 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float foamWater218 = ( 1.0 - step( largeNoisePattern297 , edgeEffect208 ) );
			float foamEdge280 = ( 1.0 - step( (0.1 + (_SinTime.w - -1.0) * (0.3 - 0.1) / (1.0 - -1.0)) , edgeEffect208 ) );
			float topWater237 = saturate( ( 1.0 - edgeEffect208 ) );
			float deepWater170 = saturate( pow( edgeEffect208 , abs( _WaterFalloff ) ) );
			half4 lerpResult13 = lerp( _BottomColor , _MidColor , deepWater170);
			o.Albedo = saturate( ( saturate( ( ( _FoamColor * foamWater218 ) + ( _FoamColor * foamEdge280 ) ) ) + ( _TopColor * topWater237 ) + lerpResult13 ) ).rgb;
			half3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			half fresnelNdotV340 = dot( ase_worldNormal, ase_worldViewDir );
			half fresnelNode340 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV340, _Smoothness ) );
			o.Smoothness = saturate( fresnelNode340 );
			half fresnelNdotV341 = dot( ase_worldNormal, ase_worldViewDir );
			half fresnelNode341 = ( 0.2 + 1.0 * pow( 1.0 - fresnelNdotV341, _Transparency ) );
			o.Alpha = saturate( fresnelNode341 );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17009
395;317;1636;1144;1877.712;1794.165;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;351;-3377.534,-2556.899;Inherit;False;1677.573;595.66;Noise Pattern;11;297;294;293;244;290;291;289;292;253;252;250;;1,0,0.02020931,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;250;-3345.259,-2318.258;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;252;-3114.005,-2317.953;Inherit;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;253;-2905.527,-2489.479;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;292;-2903.705,-2168.914;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;352;-1561.745,-2644.716;Inherit;False;1045.302;401.7801;Edge Effect;6;2;208;89;3;1;367;;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.ScaleNode;291;-2689.577,-2174.304;Inherit;False;2;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;289;-2681.025,-2482.648;Inherit;False;1;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-1530.908,-2595.276;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;290;-2499.577,-2194.304;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;244;-2505.984,-2497.292;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;367;-1518.54,-2423.202;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;1;-1310.408,-2596.476;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1085.5,-2516.533;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;293;-2255.578,-2323.303;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;294;-2128.578,-2324.303;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;353;-1566.589,-2209.598;Inherit;False;1051.563;320.1928;Edge Foam;6;329;339;274;287;278;280;;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.AbsOpNode;89;-903.6614,-2548.461;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;208;-730.2281,-2553.298;Float;False;edgeEffect;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;354;-1565.63,-1821.566;Inherit;False;1051.563;320.1928;Water Foam;5;218;232;219;298;271;;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.SinTimeNode;329;-1514.995,-2156.99;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;297;-1950.956,-2318.677;Float;False;largeNoisePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;298;-1469.535,-1739.599;Inherit;False;297;largeNoisePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;219;-1420.444,-1643.341;Inherit;False;208;edgeEffect;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;339;-1355.259,-2156.898;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.1;False;4;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;274;-1499.951,-1973.969;Inherit;False;208;edgeEffect;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;356;-1565.957,-1051.706;Inherit;False;1051.563;320.1928;Deep Water Layer;6;94;87;214;10;170;368;;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1584.74,-849.4924;Float;False;Property;_WaterFalloff;Water Falloff;10;0;Create;True;0;0;False;0;0;-0.5;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;287;-1102.779,-2077.361;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;271;-1208.314,-1694.47;Inherit;False;2;0;FLOAT;0.1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;355;-1563.427,-1433.233;Inherit;False;1051.563;320.1928;Top Water Layer;4;237;236;235;234;;0.7678117,1,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-1499.294,-973.7419;Inherit;False;208;edgeEffect;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;368;-1286.212,-842.5651;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;232;-1034.45,-1689.661;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;234;-1449.784,-1289.383;Inherit;False;208;edgeEffect;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;278;-963.7131,-2073.04;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;360;-386.3934,-1035.188;Inherit;False;1167.84;609.9176;Normal Animation;11;347;241;238;363;242;366;364;362;346;345;243;;0.8808007,0,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;87;-1138.399,-900.0184;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;235;-1239.536,-1282.809;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-838.5452,-1690.266;Float;False;foamWater;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;357;-383.0243,-2558.117;Inherit;False;873.5532;432.2462;Foam Color;7;300;296;288;222;220;285;221;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;280;-763.4899,-2076.442;Float;False;foamEdge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;285;-353.0518,-2225.498;Inherit;False;280;foamEdge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;94;-935.7496,-893.019;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;221;-354.4274,-2503.095;Float;False;Property;_FoamColor;Foam Color;6;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;243;-359.0906,-739.1885;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;0;0.025;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;236;-1023.389,-1282.796;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;220;-352.6421,-2320.392;Inherit;False;218;foamWater;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;-39.60422,-2430.212;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;345;-276.6249,-970.3161;Float;False;Constant;_Vector0;Vector 0;9;0;Create;True;0;0;False;0;100,100;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;358;-384.0041,-2065.263;Inherit;False;869.8282;370.7843;Top Color;3;209;202;204;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;170;-740.1285,-898.2707;Float;False;deepWater;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;346;-360.7276,-831.2258;Float;False;Property;_Size;Size;2;0;Create;True;0;0;False;0;0;0.25;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;237;-834.2634,-1287.29;Float;False;topWater;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;359;-380.7762,-1633.38;Inherit;False;867.4948;534.1254;Deep Color;4;171;12;13;11;;0,1,0.8708036,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-44.04728,-2298.816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;242;-62.83298,-733.438;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;169.0296,-2367.469;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;202;-310.4486,-2006.183;Float;False;Property;_TopColor;Top Color;7;0;Create;True;0;0;False;0;0,0,0,0;0.4487242,0.4716981,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;171;-299.7184,-1194.377;Inherit;False;170;deepWater;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;347;-25.13484,-917.4741;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;363;-8.35088,-571.2373;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-309.0421,-1386.396;Float;False;Property;_MidColor;Mid Color;8;0;Create;True;0;0;False;0;1,1,1,0;0.1137252,0.5294118,0.4537714,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;12;-307.5036,-1587.084;Float;False;Property;_BottomColor;Bottom Color;9;0;Create;True;0;0;False;0;0,0,0,0;0.2781996,0.0959413,0.415094,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;209;-294.1595,-1816.954;Inherit;False;237;topWater;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;213;486.0745,-45.96109;Float;False;Property;_Transparency;Transparency;4;0;Create;True;0;0;False;0;0;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;366;168.3828,-744.7504;Float;False;Property;_NormalStrenght;Normal Strenght;1;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;239;487.8338,-291.6268;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;1;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;208.409,-1428.934;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;241;191.3546,-938.6027;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;100,100;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;300;329.8889,-2365.972;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;364;197.858,-611.2705;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;100,100;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;192.3051,-1899.732;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;238;478.2904,-964.1451;Inherit;True;Property;_Normal;Normal;0;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.25;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;206;900.8405,-1638.472;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;340;888.0751,-376.476;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;341;891.0469,-133.9227;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0.2;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;362;471.4243,-643.7675;Inherit;True;Property;_Normal02;Normal 02;0;3;[HideInInspector];[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;238;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.25;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;348;1174.949,-116.3427;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;361;888.691,-751.1317;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;349;1162.126,-338.0846;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;350;1045.455,-1636.547;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1399.875,-773.275;Half;False;True;2;ASEMaterialInspector;0;0;Standard;IL3DN/Water;False;False;False;False;False;False;True;False;True;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;9;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;xboxone;ps4;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;252;0;250;0
WireConnection;253;0;252;0
WireConnection;292;0;252;0
WireConnection;291;0;292;0
WireConnection;289;0;253;0
WireConnection;290;0;291;0
WireConnection;244;0;289;0
WireConnection;1;0;2;0
WireConnection;3;0;1;0
WireConnection;3;1;367;4
WireConnection;293;0;244;0
WireConnection;293;1;290;0
WireConnection;294;0;293;0
WireConnection;89;0;3;0
WireConnection;208;0;89;0
WireConnection;297;0;294;0
WireConnection;339;0;329;4
WireConnection;287;0;339;0
WireConnection;287;1;274;0
WireConnection;271;0;298;0
WireConnection;271;1;219;0
WireConnection;368;0;10;0
WireConnection;232;0;271;0
WireConnection;278;0;287;0
WireConnection;87;0;214;0
WireConnection;87;1;368;0
WireConnection;235;0;234;0
WireConnection;218;0;232;0
WireConnection;280;0;278;0
WireConnection;94;0;87;0
WireConnection;236;0;235;0
WireConnection;222;0;221;0
WireConnection;222;1;220;0
WireConnection;170;0;94;0
WireConnection;237;0;236;0
WireConnection;288;0;221;0
WireConnection;288;1;285;0
WireConnection;242;0;243;0
WireConnection;296;0;222;0
WireConnection;296;1;288;0
WireConnection;347;0;345;0
WireConnection;347;1;346;0
WireConnection;363;0;242;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;13;2;171;0
WireConnection;241;0;347;0
WireConnection;241;1;242;0
WireConnection;300;0;296;0
WireConnection;364;0;347;0
WireConnection;364;1;363;0
WireConnection;204;0;202;0
WireConnection;204;1;209;0
WireConnection;238;1;241;0
WireConnection;238;5;366;0
WireConnection;206;0;300;0
WireConnection;206;1;204;0
WireConnection;206;2;13;0
WireConnection;340;3;239;0
WireConnection;341;3;213;0
WireConnection;362;1;364;0
WireConnection;362;5;366;0
WireConnection;348;0;341;0
WireConnection;361;0;238;0
WireConnection;361;1;362;0
WireConnection;349;0;340;0
WireConnection;350;0;206;0
WireConnection;0;0;350;0
WireConnection;0;1;361;0
WireConnection;0;4;349;0
WireConnection;0;9;348;0
ASEEND*/
//CHKSM=A7BD675493BCC7D6E8DF953DB2E92853B1C09BAD
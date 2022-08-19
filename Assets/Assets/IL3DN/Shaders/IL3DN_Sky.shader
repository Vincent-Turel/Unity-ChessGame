// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "IL3DN/Sky"
{
	Properties
	{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
		_Exposure("Exposure", Range( 0 , 8)) = 1
		[HDR]_Color0("Color 0", Color) = (1,0.9696857,0,0)
		[HDR]_Color1("Color 1", Color) = (1,0,0,0)
		_Horizon("Horizon", Range( 0 , 10)) = 5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Background"  "Queue" = "Background+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _Color1;
		uniform float4 _Color0;
		uniform float _Horizon;
		uniform half4 _TintColor;
		uniform half _Exposure;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult59 = normalize( ase_worldPos );
			float4 lerpResult32 = lerp( _Color1 , _Color0 , saturate( ( normalizeResult59.y + ( saturate( ( normalizeResult59.y * _Horizon ) ) * 0.5 ) ) ));
			o.Emission = ( lerpResult32 * _TintColor * _Exposure ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
-1911;33;1492;976;1753.148;558.4258;1.3;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;34;-1549.549,3.373046;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;59;-1340.337,3.631599;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1402.435,247.804;Float;False;Property;_Horizon;Horizon;5;0;Create;True;0;0;False;0;5;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;56;-1167.464,3.899139;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1154.504,227.9081;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;70;-908.7554,237.3862;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-938.7556,356.1862;Float;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-730.1556,230.286;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-475.3555,29.88614;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-429.5858,-200.6578;Float;False;Property;_Color1;Color 1;4;1;[HDR];Create;True;0;0;False;0;1,0,0,0;0.2028301,0.9001094,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-443.0764,-402.3075;Float;False;Property;_Color0;Color 0;3;1;[HDR];Create;True;0;0;False;0;1,0.9696857,0,0;0.1969117,0.3967405,0.5566037,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;73;-220.9558,52.28615;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-72.46886,285.0114;Half;False;Property;_TintColor;Tint Color;1;1;[HDR];Create;True;0;0;False;0;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-122.939,529.991;Half;False;Property;_Exposure;Exposure;2;0;Create;True;0;0;False;0;1;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;32;-84.68915,-9.617314;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;271.9226,-11.47069;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;31;554.2389,-56.56281;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;IL3DN/Sky;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Background;;Background;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;0;34;0
WireConnection;56;0;59;0
WireConnection;61;0;56;1
WireConnection;61;1;67;0
WireConnection;70;0;61;0
WireConnection;71;0;70;0
WireConnection;71;1;72;0
WireConnection;68;0;56;1
WireConnection;68;1;71;0
WireConnection;73;0;68;0
WireConnection;32;0;3;0
WireConnection;32;1;2;0
WireConnection;32;2;73;0
WireConnection;12;0;32;0
WireConnection;12;1;11;0
WireConnection;12;2;13;0
WireConnection;31;2;12;0
ASEEND*/
//CHKSM=22B7D801504CAFF2EE0F41358E2AB3CD9C8F67AE
Shader "Graph/Point Surface GPU"
{
	Properties
	{
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[MaterialToggle] _ColorFromWorldPosition ("Color from World Position", Float) = 0
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5

		struct Cube
		{
		    float3 Position;
			float3 Color;
		    float State;
		};
		
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		StructuredBuffer<Cube> _Cubes;
		#endif

		float _Smoothness;
		float _Resolution;
		float _Step;
		float _ColorFromWorldPosition;

		void ConfigureProcedural()
		{
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			Cube cube = _Cubes[unity_InstanceID];
			unity_ObjectToWorld = 0;
			unity_ObjectToWorld._m03_m13_m23_m33 = float4(cube.Position, 1);
			unity_ObjectToWorld._m00_m11_m22 = _Step;
			#endif
		}

		void ShaderGraphFunction_float(float3 In, out float3 Out)
		{
			Out = In;
		}

		void ShaderGraphFunction_half(half3 In, out half3 Out)
		{
			Out = In;
		}
		
		struct Input
		{
			float3 worldPos;
		};
		
		void surf(Input input, inout SurfaceOutputStandard o)
		{
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			Cube cube = _Cubes[unity_InstanceID];
			
			if (cube.State == 0)
				discard;

			o.Albedo = lerp(cube.Color, input.worldPos / _Resolution, _ColorFromWorldPosition);
			#else
			o.Albedo = input.worldPos;
			#endif
			
			o.Smoothness = _Smoothness;
		}
		
		ENDCG
	}
						
	FallBack "Diffuse"
}
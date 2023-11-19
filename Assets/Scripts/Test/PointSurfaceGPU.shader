Shader "Graph/Point Surface GPU"
{
	Properties
	{
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5

		struct Cube
		{
		    float3 Position;
		    float3 Color;
		};
		
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		StructuredBuffer<Cube> _Cubes;
		#endif

		float _Step;

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

		float _Smoothness;
		
		void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
		{
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			Cube cube = _Cubes[unity_InstanceID];
			surface.Albedo = cube.Color;
			#else
			surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
			#endif
			
			surface.Smoothness = _Smoothness;
		}
		
		ENDCG
	}
						
	FallBack "Diffuse"
}
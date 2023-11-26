Shader "Cellular Automata/3D Cell"
{
	Properties
	{
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[MaterialToggle] _ColorFromState ("Color from State", Float) = 0
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
		    int State;
		};
		
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		StructuredBuffer<Cube> _Cubes;
		#endif

		float _Smoothness;
		float _Resolution;
		float4 _Rules;

		float _ColorFromState;
		float _ColorFromWorldPosition;
		sampler2D _Ramp;

		void ConfigureProcedural()
		{
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			Cube cube = _Cubes[unity_InstanceID];
			unity_ObjectToWorld = 0;
			unity_ObjectToWorld._m03_m13_m23_m33 = float4(cube.Position, 1);
			unity_ObjectToWorld._m00_m11_m22 = 1;
			#endif
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

			if (_ColorFromWorldPosition == 1)
				o.Albedo = input.worldPos / _Resolution;
			else if (_ColorFromState == 1)
				o.Albedo = tex2D(_Ramp, float2(cube.State / (_Rules.z - 1), 0));
			else
				o.Albedo = float3(1,1,1);
			#else
				o.Albedo = input.worldPos;
			#endif
			
			o.Smoothness = _Smoothness;
		}
		
		ENDCG
	}
						
	FallBack "Diffuse"
}
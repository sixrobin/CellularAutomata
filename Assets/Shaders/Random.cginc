float random(float x) { return frac(sin(dot(x, 12.9898)) * 43758.5453); }
float random(float2 v) { return frac(sin(dot(v, float2(12.9898, 78.233))) * 43758.5453); }
float random(float3 v) { return frac(sin(dot(v, float3(12.9898, 78.233, 45.5432))) * 43758.5453); }
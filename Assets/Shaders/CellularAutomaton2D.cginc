#include "CellularAutomaton.cginc"

// Each neighbours is stepped by 0.99999 because a value less than 1 means cell is already dead, just fading.
#define SAMPLE_NEIGHBOUR(dx, dy) step(0.99999, tex2Dlod(_GridBuffer, position + float4(dx, dy, 0, 0)).x);

RWTexture2D<float4> _Result;
sampler2D _GridBuffer;
float _DecayStep;

float4 GetUV(uint3 id)
{
    return float4(id.xy / _Resolution, 0, 0);
}

int ComputeMooreNeighboursCount(float4 position)
{
    float delta = 1.0 / _Resolution; // Cells neighbour distance.
    int neighbours = 0;
    
    neighbours += SAMPLE_NEIGHBOUR(-delta, -delta);
    neighbours += SAMPLE_NEIGHBOUR(-delta, 0);
    neighbours += SAMPLE_NEIGHBOUR(-delta, delta);
    neighbours += SAMPLE_NEIGHBOUR(0, delta);
    neighbours += SAMPLE_NEIGHBOUR(delta, delta);
    neighbours += SAMPLE_NEIGHBOUR(delta, 0);
    neighbours += SAMPLE_NEIGHBOUR(delta, -delta);
    neighbours += SAMPLE_NEIGHBOUR(0, -delta);

    return neighbours;
}

int ComputeNeumannNeighboursCount(float4 position)
{
    float delta = 1.0 / _Resolution; // Cells neighbour distance.
    int neighbours = 0;

    neighbours += SAMPLE_NEIGHBOUR(-delta, 0);
    neighbours += SAMPLE_NEIGHBOUR(delta, 0);
    neighbours += SAMPLE_NEIGHBOUR(0, -delta);
    neighbours += SAMPLE_NEIGHBOUR(0, delta);

    return neighbours;
}

void ApplyBuffer(uint2 uv)
{
    float4 pos = float4(uv / _Resolution, 0, 0);
    float4 buffer = tex2Dlod(_GridBuffer, pos);
    _Result[uv] = buffer;
}
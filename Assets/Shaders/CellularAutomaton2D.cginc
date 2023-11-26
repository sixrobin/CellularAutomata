#pragma kernel ApplyBuffer

#include "CellularAutomaton.cginc"
#include "Random.cginc"

RWTexture2D<float4> _Result;
sampler2D _GridBuffer;
float _DecayStep;

int ComputeNeighboursCount(float4 position)
{
    float delta = 1.0 / _Resolution;
    int neighbours = 0;

    // Each neighbours is stepped by 0.99999 because a value less than 1 means cell is already dead, but fading.
    // Thus, we do not want to add them at all in count.
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(-delta, -delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(-delta, 0, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(-delta, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(0, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(delta, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(delta, 0, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(delta, -delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(_GridBuffer, position + float4(0, -delta, 0, 0)).x);

    return neighbours;
}

// TODO: Neumann neighbours variation.

void ApplyBuffer(uint2 uv)
{
    float4 pos = float4(uv / _Resolution, 0, 0);
    float4 buffer = tex2Dlod(_GridBuffer, pos);
    _Result[uv] = buffer;
}
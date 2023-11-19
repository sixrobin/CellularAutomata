RWTexture2D<float4> Result;
sampler2D GridBuffer;
float Resolution;
float DecayStep;

int computeNeighboursCount(float4 position)
{
    float delta = 1.0 / Resolution;
    int neighbours = 0;

    // Each neighbours is stepped by 0.99999 because a value less than 1 means cell is already dead, but fading.
    // Thus, we do not want to add them at all in count.
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(-delta, -delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(-delta, 0, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(-delta, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(0, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(delta, delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(delta, 0, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(delta, -delta, 0, 0)).x);
    neighbours += step(0.99999, tex2Dlod(GridBuffer, position + float4(0, -delta, 0, 0)).x);

    return neighbours;
}
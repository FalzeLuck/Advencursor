sampler2D TextureSampler : register(s0);

// This will control how much grayscale to apply (0.0 for full color, 1.0 for full grayscale)
float grayscaleAmount;

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    // Sample the texture color
    float4 texColor = tex2D(TextureSampler, texCoord);

    // Convert to grayscale by averaging the RGB values
    float gray = dot(texColor.rgb, float3(0.3, 0.59, 0.11));

    // Lerp between the original color and grayscale based on the grayscaleAmount
    float3 blendedColor = lerp(texColor.rgb, float3(gray, gray, gray), grayscaleAmount);

    // Maintain the alpha and apply the final blended color (multiplied by input color)
    return float4(blendedColor, texColor.a) * color;
}

technique BlendTech
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

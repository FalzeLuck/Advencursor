sampler2D TextureSampler : register(s0);

float grayscaleAmount;

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 texColor = tex2D(TextureSampler, texCoord);
    
    float gray = dot(texColor.rgb, float3(0.3, 0.59, 0.11));
    
    float3 blendedColor = lerp(texColor.rgb, float3(gray, gray, gray), grayscaleAmount);
    
    return float4(blendedColor, texColor.a) * color;
}

technique BlendTech
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

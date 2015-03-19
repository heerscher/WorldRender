
Texture2D shaderTexture;
SamplerState SampleType;

cbuffer MatrixBuffer
{
    matrix worldMatrix;
    matrix viewMatrix;
    matrix projectionMatrix;
};

struct VertexInput
{
	float3 position : POSITION;
    float3 normal : NORMAL;
	float2 texCoord : TEXCOORD0;
};

struct PixelInput
{
    float4 position : SV_POSITION;
    float4 normal : NORMAL;
	float2 texCoord : TEXCOORD0;
};

PixelInput VShader(VertexInput input)
{
    PixelInput output = (PixelInput)0;
    
	output.position = float4(input.position.xyz, 1.0f);
    output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
    output.normal = float4(input.normal, 1.0f);
	output.texCoord = input.texCoord;
    
	return output;
}

float4 PShader(PixelInput input) : SV_TARGET
{
	float4 color = float4(0.1f, 0.1f, 0.1f, 1.0f); // ambient

    float4 lightDirection = float4(0.0f, 1.0f, 0.0f, 1.0f);
	float lightIntensity = saturate(dot(normalize(input.normal), lightDirection));

    float4 lightColor = float4(1.0f, 0.9f, 0.2f, 1.0f);
	color += saturate(lightColor * lightIntensity);
	
	float4 diffuseColor = shaderTexture.Sample(SampleType, input.texCoord);
	color *= diffuseColor;

	return color;
}


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
};

struct VertexOutput
{
    float4 position : SV_POSITION;
    float4 normal : NORMAL;
};

VertexOutput VShader(VertexInput input)
{
    VertexOutput output = (VertexOutput)0;
    
	output.position = float4(input.position.xyz, 1.0f);
    output.position = mul(output.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);
    output.normal = float4(input.normal, 1.0f);
    
	return output;
}

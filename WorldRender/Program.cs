using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WorldRender
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 16)]
    struct MatrixBuffer
    {
        public SlimDX.Matrix World;
        public SlimDX.Matrix View;
        public SlimDX.Matrix Projection;
    }

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG
            // Hide mouse cursor in RELEASE mode
            Cursor.Hide();
#endif

            using (var device = new Graphics.Device())
            {
                var deltaTime = 0.0f;
                var frameTime = new Timing.Timer();
                var inputState = new Input.FormEventHandler(device.Form);
                var renderCommands = new List<Graphics.RenderCommand>(32);

                // KEY BINDING TEST
                inputState.Register("moveForward").PrimaryBinding = new Input.KeyBinding(Keys.W);
                inputState.Register("moveBackward").PrimaryBinding = new Input.KeyBinding(Keys.S);
                inputState.Register("strafeLeft").PrimaryBinding = new Input.KeyBinding(Keys.A);
                inputState.Register("strafeRight").PrimaryBinding = new Input.KeyBinding(Keys.D);
                // END TEST

                // TEST CODE
                var matrix = new MatrixBuffer();
                
                
                var camera = new Graphics.Camera();
                var cameraController = new Input.CameraController(inputState, camera);

                var cbuffer = AddTest(device, renderCommands);

                float fieldOfView = Convert.ToSingle(Math.PI) / 4.0f;
                float aspectRatio = Convert.ToSingle(device.Form.Width) / Convert.ToSingle(device.Form.Height);
                float near = 0.1f;
                float far = 1000.0f;
                matrix.World = SlimDX.Matrix.Transpose(SlimDX.Matrix.Identity);
                matrix.Projection = SlimDX.Matrix.Transpose(SlimDX.Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, near, far));
                // END TEST


                SlimDX.Windows.MessagePump.Run(device.Form, () =>
                {
                    deltaTime = frameTime.Delta();
                    inputState.UpdateState();
                    cameraController.Update(deltaTime);

                    // TEST BELOW
                    matrix.View = SlimDX.Matrix.Transpose(camera.View);
                    cbuffer.Write(ref matrix);
                    // END TEST

                    renderCommands.Sort();

                    device.Clear();
                    device.Render(renderCommands);
                    device.Present();
                });
            }
        }



        private static Graphics.ConstantBuffer<MatrixBuffer> AddTest(Graphics.Device device, List<Graphics.RenderCommand> renderCommands)
        {

            string vshadercode = @"

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
";

            var pshadercode = @"
struct PixelInput
{
    float4 position : SV_POSITION;
    float4 normal : NORMAL;
};

float4 PShader(PixelInput input) : SV_TARGET
{
	float4 color = float4(0.1f, 0.1f, 0.1f, 1.0f); // ambient

    float4 lightDirection = float4(0.0f, 1.0f, 0.0f, 1.0f);
	float lightIntensity = saturate(dot(normalize(input.normal), lightDirection));

    float4 lightColor = float4(1.0f, 0.9f, 0.2f, 1.0f);
	color += saturate(lightColor * lightIntensity);
	
	float4 diffuseColor = float4(0.1f, 0.7f, 0.1f, 1.0f);
	color *= diffuseColor;

	return color;
}
";
            var inputElements = new SlimDX.Direct3D11.InputElement[]
                {
                    new SlimDX.Direct3D11.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),//,
                    new SlimDX.Direct3D11.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0)
                    //new SlimDX.Direct3D11.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 0)
                };
            var renderTarget = device.CreateScreenRenderTarget();
            var rasterizerState = new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Back, SlimDX.Direct3D11.FillMode.Solid);
            var vertexShader = new Graphics.VertexShader(device.Handle, inputElements, vshadercode, "VShader", SlimDX.D3DCompiler.ShaderFlags.None);
            var pixelShader = new Graphics.PixelShader(device.Handle, pshadercode, "PShader", SlimDX.D3DCompiler.ShaderFlags.None);

            var cbuffer = new Graphics.ConstantBuffer<MatrixBuffer>(device.Handle);
            var cbuffers = new List<Graphics.ConstantBuffer>();
            cbuffers.Add(cbuffer);

            using (var importer = new Assimp.AssimpContext())
            {
                var scene = importer.ImportFile(@"simplecube.DAE");

                foreach (var sourceMesh in scene.Meshes)
                {
                    SlimDX.DataStream dataStream = null;
                    int bytesPerVertex = 24;
                    int vertexCount = 0;
                    IEnumerable<int> indices = null;

                    dataStream = new SlimDX.DataStream(sourceMesh.VertexCount * bytesPerVertex, true, true);
                    vertexCount = sourceMesh.VertexCount;

                    for (var i = 0; i < sourceMesh.VertexCount; ++i)
                    {
                        dataStream.Write(sourceMesh.Vertices[i]);
                        dataStream.Write(sourceMesh.Normals[i]);
                        //dataStream.Write(sourceMesh.TextureCoordinateChannels[0][i]);
                    }

                    indices = sourceMesh.GetIndices();

                    var vertexBuffer = new Graphics.VertexBuffer(device.Handle, dataStream, bytesPerVertex, vertexCount, SlimDX.Direct3D11.PrimitiveTopology.TriangleList);
                    var test = new Graphics.RenderCommand(renderTarget, rasterizerState, vertexShader, pixelShader, vertexBuffer);
                    test.VertexConstantBuffers = cbuffers;
                    //test.PixelConstantBuffers = cbuffers;
                    test.IndexBuffer = null;

                    if (indices != null && indices.Count() > 0)
                    {
                        test.IndexBuffer = new Graphics.IndexBuffer(device.Handle, indices);
                    }

                    renderCommands.Add(test);
                }
            }



            return cbuffer;
        }
    }
}

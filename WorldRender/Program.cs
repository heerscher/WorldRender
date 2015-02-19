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
#if DEBUG
            MessageBox.Show("Fire up RenderDoc if needed");
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var device = new Graphics.Device())
            {
                var deltaTime = 0.0f;
                var frameTime = new Timing.Timer();
                var inputState = new Input.FormEventHandler(device.Form);
                var renderCommands = new List<Graphics.RenderCommand>(32);


                var matrix = new MatrixBuffer();
                matrix.World = SlimDX.Matrix.Identity;
                matrix.Projection = SlimDX.Matrix.Transpose(SlimDX.Matrix.PerspectiveFovLH((float)(Math.PI) / 4.0f, device.Form.Width / device.Form.Height, 0.1f, 100000.0f));
                var camera = new Graphics.Camera();

                var cbuffer = AddTest(device, renderCommands);

                SlimDX.Windows.MessagePump.Run(device.Form, () =>
                {
                    deltaTime = frameTime.Delta();
                    inputState.UpdateState();

                    camera.MovingForwards = inputState.IsKeyDown(Keys.W);
                    camera.MovingBackwards = inputState.IsKeyDown(Keys.S);
                    camera.StrafingLeft = inputState.IsKeyDown(Keys.A);
                    camera.StrafingRight = inputState.IsKeyDown(Keys.D);
                    camera.Pitch = Convert.ToSingle(inputState.MouseDeltaY()) / 500.0f;
                    camera.Angle = Convert.ToSingle(inputState.MouseDeltaX()) / 500.0f;

                    camera.Update(deltaTime);

                    matrix.View = SlimDX.Matrix.Transpose(camera.View);
                    cbuffer.Change(ref matrix);

                    renderCommands.Sort();
                    device.Render(renderCommands);
                    System.Diagnostics.Debug.WriteLine("campos: " + camera.Position.ToString());
                });
            }
        }



        private static Graphics.ConstantBuffer<MatrixBuffer> AddTest(Graphics.Device device, List<Graphics.RenderCommand> renderCommands)
        {

            string vshadercode = @"

cbuffer MatrixBuffer
{
    float4x4 worldMatrix;
    float4x4 viewMatrix;
    float4x4 projectionMatrix;
};

struct VertexInput
{
	float3 position : POSITION;
};

float4 VShader(VertexInput input) : SV_POSITION
{
	float4 pos = float4(input.position, 1.0f);
	pos = mul(pos, viewMatrix);
	pos = mul(pos, projectionMatrix);
	return pos;
}
";

            var pshadercode = @"
float4 PShader(float4 position : SV_POSITION) : SV_Target
{
    return float4(1.0f, 0.0f, 0.0f, 1.0f);
}
";
            var inputElements = new SlimDX.Direct3D11.InputElement[]
                {
                    new SlimDX.Direct3D11.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0)//,
                    //new SlimDX.Direct3D11.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),
                    //new SlimDX.Direct3D11.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 0)
                };
            var renderTarget = device.CreateScreenRenderTarget();
            var rasterizerState = new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.None, SlimDX.Direct3D11.FillMode.Wireframe);
            var vertexShader = new Graphics.VertexShader(device.Handle, inputElements, vshadercode, "VShader", SlimDX.D3DCompiler.ShaderFlags.Debug);
            var pixelShader = new Graphics.PixelShader(device.Handle, pshadercode, "PShader", SlimDX.D3DCompiler.ShaderFlags.Debug);


            var cbuffer = new Graphics.ConstantBuffer<MatrixBuffer>(device.Handle);
            var cbuffers = new List<Graphics.ConstantBuffer>();
            cbuffers.Add(cbuffer);

            using (var importer = new Assimp.AssimpContext())
            {
                var scene = importer.ImportFile(@"C:\Users\Heerscher\Desktop\nzrtxgnt35-Abandoned_House1\3ds file.3DS");

                foreach (var sourceMesh in scene.Meshes)
                {
                    SlimDX.DataStream dataStream = null;
                    int bytesPerVertex = 12;
                    int vertexCount = 0;
                    IEnumerable<int> indices = null;

                    dataStream = new SlimDX.DataStream(sourceMesh.VertexCount * bytesPerVertex, true, true);
                    vertexCount = sourceMesh.VertexCount;

                    for (var i = 0; i < sourceMesh.VertexCount; ++i)
                    {
                        var vertex = sourceMesh.Vertices[i];
                        //var normal = sourceMesh.Normals[i];
                        //var texCoord = sourceMesh.TextureCoordinateChannels[0][i];
                        dataStream.Write(new SlimDX.Vector3(vertex.X, vertex.Y, vertex.Z));
                        //dataStream.Write(new SlimDX.Vector3(normal.X, normal.Y, normal.Z));
                       // dataStream.Write(new SlimDX.Vector2(texCoord.X, texCoord.Y));
                    }

                    indices = sourceMesh.GetIndices();

                    var vertexBuffer = new Graphics.VertexBuffer(device.Handle, dataStream, bytesPerVertex, vertexCount, SlimDX.Direct3D11.PrimitiveTopology.TriangleList);
                    var test = new Graphics.RenderCommand(renderTarget, rasterizerState, vertexShader, pixelShader, vertexBuffer);
                    test.VertexConstantBuffers = cbuffers;
                    test.PixelConstantBuffers = cbuffers;
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

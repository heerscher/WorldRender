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
                var resourceCache = new Resources.Cache();
                var renderCommands = new List<Graphics.RenderCommand>(32);

                // Map resource loaders to specific types
                resourceCache.RegisterLoader(typeof(Graphics.Mesh), new Resources.Loaders.MeshLoader(device));
                resourceCache.RegisterLoader(typeof(Graphics.VertexShader), new Resources.Loaders.VertexShaderLoader(device));
                resourceCache.RegisterLoader(typeof(Graphics.PixelShader), new Resources.Loaders.PixelShaderLoader(device));


                // KEY BINDING TEST
                // TODO: this should come from some config file
                inputState.Register("moveForward").PrimaryBinding = new Input.KeyBinding(Keys.W);
                inputState.Register("moveBackward").PrimaryBinding = new Input.KeyBinding(Keys.S);
                inputState.Register("strafeLeft").PrimaryBinding = new Input.KeyBinding(Keys.A);
                inputState.Register("strafeRight").PrimaryBinding = new Input.KeyBinding(Keys.D);
                // END TEST


                // TEST CODE
                var matrix = new MatrixBuffer();
                var camera = new Graphics.Camera();
                var cameraController = new Input.CameraController(inputState, camera);

                var cbuffer = AddTest(device, resourceCache, renderCommands);

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


        private static Graphics.ConstantBuffer<MatrixBuffer> AddTest(Graphics.Device device, Resources.Cache cache, List<Graphics.RenderCommand> renderCommands)
        {
            var renderTarget = device.CreateScreenRenderTarget();
            var rasterizerState = new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Back, SlimDX.Direct3D11.FillMode.Solid);

            var vertexShader = cache.Get<Graphics.VertexShader>("shader.vs");
            var pixelShader = cache.Get<Graphics.PixelShader>("shader.ps");

            var cbuffer = new Graphics.ConstantBuffer<MatrixBuffer>(device.Handle);
            var cbuffers = new List<Graphics.ConstantBuffer>();
            cbuffers.Add(cbuffer);

            var simplecubemesh = cache.Get<Graphics.Mesh>("simplecube.DAE");
            var test = new Graphics.RenderCommand(renderTarget, rasterizerState, vertexShader, pixelShader, simplecubemesh.VertexBuffer);
            test.VertexConstantBuffers = cbuffers;

            test.IndexBuffer = simplecubemesh.IndexBuffer;

            renderCommands.Add(test);

            return cbuffer;
        }
    }
}

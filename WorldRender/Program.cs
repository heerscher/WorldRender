using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WorldRender
{
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
                using (var resourceCache = device.CreateResourceCache())
                {
                    var deltaTime = 0.0f;
                    var frameTime = new Timing.Timer();
                    var inputState = new Input.FormEventHandler(device.Form);
                    var entities = new Entities.EntityCollection();
                    var fieldOfView = Convert.ToSingle(Math.PI) / 4.0f;
                    var aspectRatio = Convert.ToSingle(device.Form.Width) / Convert.ToSingle(device.Form.Height);
                    var near = 0.1f;
                    var far = 1000.0f;
                    var projection = SlimDX.Matrix.Transpose(SlimDX.Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, near, far));
                    var camera = new Graphics.Camera();
                    var cameraController = new Input.CameraController(inputState, camera);
                    var renderCommands = new List<Graphics.RenderCommand>(1024);

                    var keyBindingConfigurationFile = new Configuration.ConfigurationFile<Configuration.KeyBindingConfiguration>("keybindings.json");
                    var keyBindingConfiguration = keyBindingConfigurationFile.Read();
                    if (keyBindingConfiguration != null)
                    {
                        keyBindingConfiguration.RegisterBindingsToInputState(inputState);
                    }


                    var testEntity = CreateTestEntity(device, resourceCache, entities);

                    var testEntity2 = CreateTestEntity(device, resourceCache, entities);
                    testEntity2.Transform.Translate(new SlimDX.Vector3(1, 1, 1));


                    SlimDX.Windows.MessagePump.Run(device.Form, () =>
                    {
                        deltaTime = frameTime.Delta();
                        inputState.UpdateState();
                        cameraController.Update(deltaTime);

                        // Create the view matrix from camera parameters
                        var view = SlimDX.Matrix.Transpose(camera.View);

                        // Clears various buffers, including the screen
                        device.Clear();
                        
                        // Convert renderable entities into render commands and sort them
                        renderCommands.Clear();
                        renderCommands.AddRange(entities.GetComponents<Entities.Components.RenderComponent>().Select(r => UpdateRenderCommandConstants(r, ref view, ref projection)));
                        renderCommands.Sort();

                        // Executes the render commands and renders the output to screen
                        device.Render(renderCommands);
                        device.Present();
                    });
                }
            }
        }

        private static Graphics.RenderCommand UpdateRenderCommandConstants(Entities.Components.RenderComponent renderComponent, ref SlimDX.Matrix view, ref SlimDX.Matrix projection)
        {
            renderComponent.RenderCommand.VertexConstants.World = renderComponent.Entity.Transform.Matrix;
            renderComponent.RenderCommand.VertexConstants.View = view;
            renderComponent.RenderCommand.VertexConstants.Projection = projection;

            return renderComponent.RenderCommand;
        }


        private static Entities.Entity CreateTestEntity(Graphics.Device device, Resources.Cache cache, Entities.EntityCollection entities)
        {
            var entity = entities.CreateEntity();
            var renderComponent = entity.AddComponent<Entities.Components.RenderComponent>();
            var material = cache.Get<Graphics.Materials.Material>("default");

            renderComponent.RenderCommand = new Graphics.RenderCommand(cache)
            {
                Mesh = cache.Get<Graphics.Mesh>("simplecube.DAE"),
                RasterizerState = material.GetRasterizerState(cache),
                Shader = material.GetShader(cache),
                Texture = cache.Get<Graphics.Texture2d>("uv_map_reference.jpg")
            };

            return entity;
        }
    }
}

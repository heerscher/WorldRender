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

                    var keyBindingConfigurationFile = new Configuration.ConfigurationFile<Configuration.KeyBindingConfiguration>("keybindings.json");
                    var keyBindingConfiguration = keyBindingConfigurationFile.Read();
                    if (keyBindingConfiguration == null && keyBindingConfiguration.Bindings != null)
                    {
                        foreach (var keyBinding in keyBindingConfiguration.Bindings)
                        {
                            var command = inputState.Register(keyBinding.Command);

                            if (keyBinding.PrimaryKey.HasValue)
                            {
                                command.PrimaryBinding = new Input.KeyBinding(keyBinding.PrimaryKey.Value);
                            }

                            if (keyBinding.SecondaryKey.HasValue)
                            {
                                command.SecondaryBinding = new Input.KeyBinding(keyBinding.SecondaryKey.Value);
                            }
                        }
                    }


                    var testEntity = CreateTestEntity(device, resourceCache, entities);


                    SlimDX.Windows.MessagePump.Run(device.Form, () =>
                    {
                        deltaTime = frameTime.Delta();
                        inputState.UpdateState();
                        cameraController.Update(deltaTime);

                        var view = SlimDX.Matrix.Transpose(camera.View);

                        device.Clear();
                        device.Render(entities.Render(ref view, ref projection));

                        device.Present();
                    });
                }
            }
        }


        private static Entities.Entity CreateTestEntity(Graphics.Device device, Resources.Cache cache, Entities.EntityCollection entities)
        {
            var entity = entities.CreateEntity();
            var renderComponent = entity.AddComponent<Entities.Components.RenderComponent>();

            var renderTarget = device.CreateScreenRenderTarget();
            var rasterizerState = cache.Get<Graphics.RasterizerState>("default");

            var vertexShader = cache.Get<Graphics.VertexShader>("shader.vs");
            var pixelShader = cache.Get<Graphics.PixelShader>("shader.ps");

            var simplecubemesh = cache.Get<Graphics.Mesh>("simplecube.DAE");
            var renderCommand = renderComponent.CreateCommand(device, renderTarget, rasterizerState, vertexShader, pixelShader, simplecubemesh);

            renderCommand.Texture = cache.Get<Graphics.Texture>("uv_map_reference.jpg");

            return entity;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender
{
    public class GameInstance : IDisposable
    {
        
        private Input.CameraController cameraController;
        private float deltaTime;
        private Graphics.Device device;
        private Timing.Timer frameTime;
        private Input.IState inputState;
        private Resources.Cache resourceCache;

        public Graphics.Device Device
        {
            get
            {
                return device;
            }
        }

        public Input.IState InputState
        {
            get
            {
                return inputState;
            }
        }

        public Resources.Cache ResourceCache
        {
            get
            {
                return resourceCache;
            }
        }

        public Scene.Scene Scene { get; set; }

        public GameInstance()
        {
            device = new Graphics.Device();
            frameTime = new Timing.Timer();
            inputState = new Input.FormEventHandler(device.Form);
            resourceCache = device.CreateResourceCache();           
            
            var keyBindingConfigurationFile = new Configuration.ConfigurationFile<Configuration.KeyBindingConfiguration>("keybindings.json");
            var keyBindingConfiguration = keyBindingConfigurationFile.Read();
            if (keyBindingConfiguration != null)
            {
                keyBindingConfiguration.RegisterBindingsToInputState(inputState);
            }
        }

        public void Run()
        {
            SlimDX.Windows.MessagePump.Run(device.Form, () =>
            {
                deltaTime = frameTime.Delta();
                inputState.UpdateState();

                if (Scene != null)
                {
                    Scene.OnUpdate(deltaTime);
                }

                // Clears various buffers, including the screen
                device.Clear();

                if (Scene != null)
                {
                    Scene.Render();
                }

                device.Present();
            });
        }

        public void Dispose()
        {
            if (resourceCache != null)
            {
                resourceCache.Dispose();
                resourceCache = null;
            }

            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }
    }
}

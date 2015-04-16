using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender
{
    /// <summary>
    /// Represents an instance of a game.
    /// Brings together different parts of the engine and contains the game's mainloop.
    /// </summary>
    public class GameInstance : IDisposable
    {
        private float deltaTime;
        private Graphics.Device device;
        private Timing.Timer frameTime;
        private Input.IState inputState;

        /// <summary>
        /// Gets the device responsible for rendering the game to the screen.
        /// </summary>
        public Graphics.Device Device
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// Gets the state of input devices.
        /// </summary>
        public Input.IState Input
        {
            get
            {
                return inputState;
            }
        }

        public Scene.Scene Scene { get; set; }

        public GameInstance()
        {
            device = new Graphics.Device();
            frameTime = new Timing.Timer();
            inputState = new Input.FormEventHandler(device.Form);
            
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
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }
    }
}

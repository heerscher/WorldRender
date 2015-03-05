using System;
using System.Collections.Generic;

namespace WorldRender.Configuration
{
    [Serializable]
    public class KeyBindingConfiguration
    {
        public IEnumerable<KeyBindingConfigurationItem> Bindings { get; set; }

        public void RegisterBindingsToInputState(Input.IState inputState)
        {
#if ASSERT
            if (inputState == null)
            {
                throw new ArgumentNullException("inputState");
            }
#endif

            if (Bindings != null)
            {
                foreach (var keyBinding in Bindings)
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
        }
    }
}

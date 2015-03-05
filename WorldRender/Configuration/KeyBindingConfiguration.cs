using System;
using System.Collections.Generic;

namespace WorldRender.Configuration
{
    [Serializable]
    public class KeyBindingConfiguration
    {
        public IEnumerable<KeyBindingConfigurationItem> Bindings { get; set; }
    }
}

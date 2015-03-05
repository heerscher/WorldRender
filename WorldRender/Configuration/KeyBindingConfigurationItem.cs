using System;

namespace WorldRender.Configuration
{
    [Serializable]
    public class KeyBindingConfigurationItem
    {
        public string Command { get; set; }
        public System.Windows.Forms.Keys? PrimaryKey { get; set; }
        public System.Windows.Forms.Keys? SecondaryKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources.Loaders
{
    public class TextureLoader : Loader
    {
        private Graphics.Device device;
        private IEnumerable<Type> supportedTypes;

        public TextureLoader(Graphics.Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            this.device = device;

            supportedTypes = new Type[]
            {
                typeof(Graphics.Texture)
            };
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return supportedTypes;
            }
        }

        public override IDisposable Load(Type resourceType, string identifier)
        {
            return new Graphics.Texture(device.Handle, identifier);
        }
    }
}

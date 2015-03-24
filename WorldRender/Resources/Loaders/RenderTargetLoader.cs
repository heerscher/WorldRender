using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources.Loaders
{
    public class RenderTargetLoader : BaseLoader
    {
        private Graphics.Device device;

        public RenderTargetLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.RenderTarget)
            })
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            this.device = device;
        }

        public override IDisposable Load(Type resourceType, string identifier)
        {
            if (string.Equals(identifier, "screen", StringComparison.OrdinalIgnoreCase))
            {
                return device.CreateScreenRenderTarget();
            }
            else
            {
                // TODO: support render targets (ie for deferred rendering)
                throw new NotSupportedException("Not supported yet");
            }
        }
    }
}

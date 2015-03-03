using System;
using System.Collections.Generic;

namespace WorldRender.Resources.Loaders
{
    public class RasterizerStateLoader : Loader
    {
        private Graphics.Device device;
        private IEnumerable<Type> supportedTypes;

        public RasterizerStateLoader(Graphics.Device device)
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
                typeof(Graphics.RasterizerState)
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
            switch (identifier)
            {
                case "back;wireframe":
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Back, SlimDX.Direct3D11.FillMode.Wireframe);
                case "front;solid":
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Front, SlimDX.Direct3D11.FillMode.Solid);
                case "none;solid":
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.None, SlimDX.Direct3D11.FillMode.Solid);
                case "front;wireframe":
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Front, SlimDX.Direct3D11.FillMode.Wireframe);
                case "none;wireframe":
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.None, SlimDX.Direct3D11.FillMode.Wireframe);
                default:
                    return new Graphics.RasterizerState(device.Handle, SlimDX.Direct3D11.CullMode.Back, SlimDX.Direct3D11.FillMode.Solid);
            }
        }
    }
}

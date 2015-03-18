using System;

namespace WorldRender.Resources.Loaders
{
    public class RasterizerStateLoader : BaseLoader
    {
        private Graphics.Device device;

        public RasterizerStateLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.RasterizerState)
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

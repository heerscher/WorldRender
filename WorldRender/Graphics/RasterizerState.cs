using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class RasterizerState : IDisposable
    {
        private SlimDX.Direct3D11.RasterizerState rasterizerState;
        private bool resourceOwner;
        private UniqueId<RasterizerState> uniqueId;

        internal int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        internal RasterizerState(SlimDX.Direct3D11.Device device, SlimDX.Direct3D11.CullMode cullMode, SlimDX.Direct3D11.FillMode fillMode)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            var rasterizerStateDescription = new SlimDX.Direct3D11.RasterizerStateDescription()
            {
                CullMode = cullMode,
                FillMode = fillMode
            };

            rasterizerState = SlimDX.Direct3D11.RasterizerState.FromDescription(device, rasterizerStateDescription);
            resourceOwner = true;
            uniqueId = new UniqueId<RasterizerState>();
        }

        public void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }
#endif

            deviceContext.Rasterizer.State = rasterizerState;
        }

        public void Dispose()
        {
            if (resourceOwner && rasterizerState != null)
            {
                rasterizerState.Dispose();
            }

            rasterizerState = null;
        }
    }
}

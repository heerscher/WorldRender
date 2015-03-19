using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class Texture2d : IDisposable
    {
        private SlimDX.Direct3D11.Texture2D texture2d;
        private SlimDX.Direct3D11.ShaderResourceView shaderResourceView;
        private UniqueId<Texture2d> uniqueId;

        internal int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        internal Texture2d(SlimDX.Direct3D11.Device device, string fileName)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            texture2d = SlimDX.Direct3D11.Texture2D.FromFile(device, fileName);
            shaderResourceView = new SlimDX.Direct3D11.ShaderResourceView(device, texture2d);
            uniqueId = new UniqueId<Texture2d>();
        }

        internal void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
            deviceContext.PixelShader.SetShaderResource(shaderResourceView, 0);
        }

        public void Dispose()
        {
            if (shaderResourceView != null)
            {
                shaderResourceView.Dispose();
                shaderResourceView = null;
            }

            if (texture2d != null)
            {
                texture2d.Dispose();
                texture2d = null;
            }
        }
    }
}

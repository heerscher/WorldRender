using System;

namespace WorldRender.Graphics
{
    public class PixelShader : IDisposable
    {
        private bool resourceOwner;
        private SlimDX.Direct3D11.PixelShader pixelShader;

        internal PixelShader(SlimDX.Direct3D11.PixelShader pixelShader)
        {
#if ASSERT
            if (pixelShader == null)
            {
                throw new ArgumentNullException("pixelShader");
            }
#endif

            resourceOwner = false;
            this.pixelShader = pixelShader;
        }

        internal PixelShader(SlimDX.Direct3D11.Device device, SlimDX.D3DCompiler.ShaderBytecode pixelShaderCode)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (pixelShaderCode == null)
            {
                throw new ArgumentNullException("pixelShaderCode");
            }
#endif

            resourceOwner = true;
            pixelShader = new SlimDX.Direct3D11.PixelShader(device, pixelShaderCode);
        }

        internal PixelShader(SlimDX.Direct3D11.Device device, string shaderCode, string entryPoint, SlimDX.D3DCompiler.ShaderFlags shaderFlags)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (shaderCode == null)
            {
                throw new ArgumentNullException("shaderCode");
            }

            if (shaderCode.Length == 0)
            {
                throw new ArgumentOutOfRangeException("shaderCode");
            }

            if (entryPoint == null)
            {
                throw new ArgumentNullException("entryPoint");
            }

            if (entryPoint.Length == 0)
            {
                throw new ArgumentOutOfRangeException("entryPoint");
            }
#endif

            resourceOwner = true;

            using (var pixelShaderCode = SlimDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, entryPoint, "ps_5_0", shaderFlags, SlimDX.D3DCompiler.EffectFlags.None))
            {
                pixelShader = new SlimDX.Direct3D11.PixelShader(device, pixelShaderCode);
            }
        }

        internal void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }
#endif

            deviceContext.PixelShader.Set(pixelShader);
        }

        public void Dispose()
        {
            if (resourceOwner && pixelShader != null)
            {
                pixelShader.Dispose();
            }

            pixelShader = null;
        }
    }
}

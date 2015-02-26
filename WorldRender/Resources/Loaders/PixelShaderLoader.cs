using System;

namespace WorldRender.Resources.Loaders
{
    public class PixelShaderLoader : Loader
    {
        private Graphics.Device device;

        public PixelShaderLoader(Graphics.Device device)
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
            var shaderCode = System.IO.File.ReadAllText(identifier);

            return new Graphics.PixelShader(device.Handle, shaderCode, "PShader", SlimDX.D3DCompiler.ShaderFlags.None);
        }
    }
}

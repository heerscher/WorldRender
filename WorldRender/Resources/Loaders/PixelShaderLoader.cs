using System;

namespace WorldRender.Resources.Loaders
{
    public class PixelShaderLoader : BaseLoader
    {
        private Graphics.Device device;

        public PixelShaderLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.Shaders.PixelShader)
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
            var shaderCode = System.IO.File.ReadAllText(identifier);

            return new Graphics.Shaders.PixelShader(device.Handle, shaderCode, "PShader", SlimDX.D3DCompiler.ShaderFlags.None);
        }
    }
}

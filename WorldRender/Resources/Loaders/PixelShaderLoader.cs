using System;
using System.Collections.Generic;

namespace WorldRender.Resources.Loaders
{
    public class PixelShaderLoader : Loader
    {
        private Graphics.Device device;
        private IEnumerable<Type> supportedTypes;

        public PixelShaderLoader(Graphics.Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            this.device = device;
            supportedTypes = new Type[] { typeof(Graphics.PixelShader) };
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
            var shaderCode = System.IO.File.ReadAllText(identifier);

            return new Graphics.PixelShader(device.Handle, shaderCode, "PShader", SlimDX.D3DCompiler.ShaderFlags.None);
        }
    }
}

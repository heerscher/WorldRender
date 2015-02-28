using System;
using System.Collections.Generic;

namespace WorldRender.Resources.Loaders
{
    class VertexShaderLoader : Loader
    {
        private Graphics.Device device;
        private IEnumerable<Type> supportedTypes;

        public VertexShaderLoader(Graphics.Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            this.device = device;
            supportedTypes = new Type[] { typeof(Graphics.VertexShader) };
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

            var inputElements = new SlimDX.Direct3D11.InputElement[]
            {
                new SlimDX.Direct3D11.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),
                new SlimDX.Direct3D11.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),
                new SlimDX.Direct3D11.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 0)
            };
            
            return new Graphics.VertexShader(device.Handle, inputElements, shaderCode, "VShader", SlimDX.D3DCompiler.ShaderFlags.None);
        }
    }
}

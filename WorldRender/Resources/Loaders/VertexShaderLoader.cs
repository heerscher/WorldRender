using System;
using System.Collections.Generic;

namespace WorldRender.Resources.Loaders
{
    public class VertexShaderLoader : BaseLoader
    {
        private Graphics.Device device;

        public VertexShaderLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.VertexShader)
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

            var inputElements = new SlimDX.Direct3D11.InputElement[]
            {
                new SlimDX.Direct3D11.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),
                new SlimDX.Direct3D11.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0),
                new SlimDX.Direct3D11.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 0)
            };
            
            var vertexShader = new Graphics.VertexShader(device.Handle, inputElements, shaderCode, "VShader", SlimDX.D3DCompiler.ShaderFlags.None);

            if (resourceType.Equals(typeof(Graphics.Effect)))
            {
                return new Graphics.Effect(vertexShader, new Graphics.PixelShader(device.Handle, shaderCode, "PShader", SlimDX.D3DCompiler.ShaderFlags.None));
            }

            return vertexShader;
        }
    }
}

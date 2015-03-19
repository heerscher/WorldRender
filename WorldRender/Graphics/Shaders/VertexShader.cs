using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics.Shaders
{
    public class VertexShader : IDisposable
    {
        private SlimDX.Direct3D11.InputLayout inputLayout;
        private bool resourceOwner;
        private SlimDX.Direct3D11.VertexShader vertexShader;
        private UniqueId<VertexShader> uniqueId;

        internal int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        internal VertexShader(SlimDX.Direct3D11.InputLayout inputLayout, SlimDX.Direct3D11.VertexShader vertexShader)
        {
#if ASSERT
            if (inputLayout == null)
            {
                throw new ArgumentNullException("inputLayout");
            }

            if (vertexShader == null)
            {
                throw new ArgumentNullException("vertexShader");
            }
#endif

            this.inputLayout = inputLayout;
            resourceOwner = false;
            this.vertexShader = vertexShader;
            uniqueId = new UniqueId<VertexShader>();
        }

        internal VertexShader(SlimDX.Direct3D11.Device device, IEnumerable<SlimDX.Direct3D11.InputElement> inputElements, SlimDX.D3DCompiler.ShaderBytecode vertexShaderCode)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (inputElements == null)
            {
                throw new ArgumentNullException("inputElements");
            }

            if (vertexShaderCode == null)
            {
                throw new ArgumentNullException("vertexShaderCode");
            }
#endif

            resourceOwner = true;
            vertexShader = new SlimDX.Direct3D11.VertexShader(device, vertexShaderCode);
            inputLayout = new SlimDX.Direct3D11.InputLayout(device, vertexShaderCode, inputElements.ToArray());
            uniqueId = new UniqueId<VertexShader>();
        }

        internal VertexShader(SlimDX.Direct3D11.Device device, IEnumerable<SlimDX.Direct3D11.InputElement> inputElements, string shaderCode, string entryPoint, SlimDX.D3DCompiler.ShaderFlags shaderFlags)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (inputElements == null)
            {
                throw new ArgumentNullException("inputElements");
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
            uniqueId = new UniqueId<VertexShader>();

            using (var vertexShaderCode = SlimDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, entryPoint, "vs_5_0", shaderFlags, SlimDX.D3DCompiler.EffectFlags.None))
            {
                vertexShader = new SlimDX.Direct3D11.VertexShader(device, vertexShaderCode);
                inputLayout = new SlimDX.Direct3D11.InputLayout(device, vertexShaderCode, inputElements.ToArray());
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

            deviceContext.InputAssembler.InputLayout = inputLayout;

            deviceContext.VertexShader.Set(vertexShader);
        }

        public void Dispose()
        {
            if (resourceOwner)
            {
                if (inputLayout != null)
                {
                    inputLayout.Dispose();
                }

                if (vertexShader != null)
                {
                    vertexShader.Dispose();
                }
            }

            inputLayout = null;
            vertexShader = null;
        }
    }
}

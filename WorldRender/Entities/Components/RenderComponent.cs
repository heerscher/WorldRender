using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities.Components
{
    public class RenderComponent : Component
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 16)]
        private struct VertexConstantBuffer
        {
            public SlimDX.Matrix World;
            public SlimDX.Matrix View;
            public SlimDX.Matrix Projection;
        }

        private VertexConstantBuffer buffer;
        private Graphics.ConstantBuffer<VertexConstantBuffer> constantBuffer;
        private Graphics.RenderCommand renderCommand;

        public RenderComponent(Entity entity)
            : base(entity)
        {
        }

        public Graphics.RenderCommand Render(ref SlimDX.Matrix view, ref SlimDX.Matrix projection)
        {
            if (constantBuffer != null)
            {
                var transformComponent = Entity.GetComponent<TransformComponent>();

                buffer.World = transformComponent.Matrix;
                buffer.View = view;
                buffer.Projection = projection;

                constantBuffer.Write(ref buffer);
            }

            return renderCommand;
        }

        public Graphics.RenderCommand CreateCommand(Graphics.Device device, Graphics.RenderTarget renderTarget, Graphics.RasterizerState rasterizerState, Graphics.VertexShader vertexShader, Graphics.PixelShader pixelShader, Graphics.Mesh mesh)
        {
            CreateCommand(device, renderTarget, rasterizerState, vertexShader, pixelShader, mesh.VertexBuffer);

            renderCommand.IndexBuffer = mesh.IndexBuffer;

            return renderCommand;
        }

        public Graphics.RenderCommand CreateCommand(Graphics.Device device, Graphics.RenderTarget renderTarget, Graphics.RasterizerState rasterizerState, Graphics.VertexShader vertexShader, Graphics.PixelShader pixelShader, Graphics.VertexBuffer vertexBuffer)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (renderTarget == null)
            {
                throw new ArgumentNullException("renderTarget");
            }

            if (rasterizerState == null)
            {
                throw new ArgumentNullException("rasterizerState");
            }

            if (vertexShader == null)
            {
                throw new ArgumentNullException("vertexShader");
            }

            if (pixelShader == null)
            {
                throw new ArgumentNullException("pixelShader");
            }

            if (vertexBuffer == null)
            {
                throw new ArgumentNullException("vertexBuffer");
            }
#endif

            renderCommand = new Graphics.RenderCommand(renderTarget, rasterizerState, vertexShader, pixelShader, vertexBuffer);
            constantBuffer = new Graphics.ConstantBuffer<VertexConstantBuffer>(device.Handle);
            renderCommand.VertexConstantBuffers = new Graphics.ConstantBuffer[] { constantBuffer };

            return renderCommand;
        }
    }
}

using System;
using System.Collections.Generic;

namespace WorldRender.Graphics
{
    public class RenderCommand : IComparable<RenderCommand>
    {
        private RasterizerState rasterizerState;
        private RenderTarget renderTarget;
        private PixelShader pixelShader;
        private UInt64 sortKey;
        private VertexBuffer vertexBuffer;
        private VertexShader vertexShader;

        public IndexBuffer IndexBuffer { get; set; }
        public IEnumerable<ConstantBuffer> PixelConstantBuffers { get; set; }
        public IEnumerable<ConstantBuffer> VertexConstantBuffers { get; set; }

        public RenderCommand(RenderTarget renderTarget, RasterizerState rasterizerState, VertexShader vertexShader, PixelShader pixelShader, VertexBuffer vertexBuffer)
        {
#if ASSERT
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

            this.rasterizerState = rasterizerState;
            this.renderTarget = renderTarget;
            this.pixelShader = pixelShader;
            this.vertexBuffer = vertexBuffer;
            this.vertexShader = vertexShader;

            // Calculate sortkey
            sortKey = 0UL;
            sortKey += Convert.ToUInt64(renderTarget.Id) << 60;
            sortKey += Convert.ToUInt64(vertexShader.Id) << 56;
            sortKey += Convert.ToUInt64(pixelShader.Id) << 52;
            sortKey += Convert.ToUInt64(rasterizerState.Id) << 48;
        }

        public void Render(Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            renderTarget.Render(device.Context);
            vertexShader.Render(device.Context);

            if (VertexConstantBuffers != null)
            {
                var slot = 0;

                foreach (var constantBuffer in VertexConstantBuffers)
                {
                    constantBuffer.UpdateVertexShader(device.Context, slot);

                    ++slot;
                }
            }

            pixelShader.Render(device.Context);

            if (PixelConstantBuffers != null)
            {
                var slot = 0;

                foreach (var constantBuffer in PixelConstantBuffers)
                {
                    constantBuffer.UpdatePixelShader(device.Context, slot);

                    ++slot;
                }
            }

            rasterizerState.Render(device.Context);
            vertexBuffer.Render(device.Context);

            if (IndexBuffer == null)
            {
                device.Context.Draw(vertexBuffer.Count, 0);
            }
            else
            {
                IndexBuffer.Render(device.Context);

                device.Context.DrawIndexed(IndexBuffer.Count, 0, 0);
            }
        }

        public int CompareTo(RenderCommand other)
        {
            return Convert.ToInt32(sortKey - other.sortKey);
        }
    }
}

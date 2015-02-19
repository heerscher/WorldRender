using System;

namespace WorldRender.Graphics
{
    public class VertexBuffer : IDisposable
    {
        private int bytesPerVertex;
        private SlimDX.Direct3D11.PrimitiveTopology primitiveTopology;
        private bool resourceOwner;
        private SlimDX.Direct3D11.Buffer vertexBuffer;
        private SlimDX.Direct3D11.VertexBufferBinding vertexBufferBinding;
        private int vertexCount;

        /// <summary>
        /// Gets the amount of vertices stored inside this buffer.
        /// </summary>
        public int Count
        {
            get
            {
                return vertexCount;
            }
        }

        internal VertexBuffer(SlimDX.Direct3D11.Device device, SlimDX.DataStream dataStream, int bytesPerVertex, int vertexCount)
            : this(device, dataStream, bytesPerVertex, vertexCount, SlimDX.Direct3D11.PrimitiveTopology.TriangleList)
        {
        }

        internal VertexBuffer(SlimDX.Direct3D11.Device device, SlimDX.DataStream dataStream, int bytesPerVertex, int vertexCount, SlimDX.Direct3D11.PrimitiveTopology primitiveTopology)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (dataStream == null)
            {
                throw new ArgumentNullException("dataStream");
            }

            if (bytesPerVertex <= 0)
            {
                throw new ArgumentOutOfRangeException("bytesPerVertex");
            }

            if (vertexCount <= 0)
            {
                throw new ArgumentOutOfRangeException("vertexCount");
            }
#endif

            this.bytesPerVertex = bytesPerVertex;
            this.primitiveTopology = primitiveTopology;
            this.resourceOwner = true;
            this.vertexCount = vertexCount;

            // Seek to the start of the vertices
            dataStream.Position = 0;

            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, dataStream, vertexCount * bytesPerVertex, SlimDX.Direct3D11.ResourceUsage.Default, SlimDX.Direct3D11.BindFlags.VertexBuffer,
                    SlimDX.Direct3D11.CpuAccessFlags.None, SlimDX.Direct3D11.ResourceOptionFlags.None, 0);
            vertexBufferBinding = new SlimDX.Direct3D11.VertexBufferBinding(vertexBuffer, bytesPerVertex, 0);
        }

        internal void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }
#endif

            deviceContext.InputAssembler.PrimitiveTopology = primitiveTopology;

            deviceContext.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
        }

        public void Dispose()
        {
            if (resourceOwner && vertexBuffer != null)
            {
                vertexBuffer.Dispose();
            }

            vertexBuffer = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class Mesh : IDisposable
    {
        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;

        public IndexBuffer IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
        }

        public VertexBuffer VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }

        public Mesh(VertexBuffer vertexBuffer)
        {
#if ASSERT
            if (vertexBuffer == null)
            {
                throw new ArgumentNullException("vertexBuffer");
            }
#endif

            this.vertexBuffer = vertexBuffer;
        }

        public Mesh(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
#if ASSERT
            if (vertexBuffer == null)
            {
                throw new ArgumentNullException("vertexBuffer");
            }
#endif

            this.indexBuffer = indexBuffer;
            this.vertexBuffer = vertexBuffer;
        }

        public void Dispose()
        {
            if (indexBuffer != null)
            {
                indexBuffer.Dispose();
            }

            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
            }

            indexBuffer = null;
            vertexBuffer = null;
        }
    }
}

using System;

namespace WorldRender.Graphics
{
    public class Effect : IDisposable
    {
        private PixelShader pixelShader;
        private VertexShader vertexShader;

        public PixelShader PixelShader
        {
            get
            {
                return pixelShader;
            }
        }

        public VertexShader VertexShader
        {
            get
            {
                return vertexShader;
            }
        }

        public Effect(VertexShader vertexShader, PixelShader pixelShader)
        {
#if ASSERT
            if (vertexShader == null)
            {
                throw new ArgumentNullException("vertexShader");
            }

            if (pixelShader == null)
            {
                throw new ArgumentNullException("pixelShader");
            }
#endif

            this.pixelShader = pixelShader;
            this.vertexShader = vertexShader;
        }

        public void Dispose()
        {
            if (pixelShader != null)
            {
                pixelShader.Dispose();
            }

            if (vertexShader != null)
            {
                vertexShader.Dispose();
            }

            pixelShader = null;
            vertexShader = null;
        }
    }
}

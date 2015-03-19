using System;

namespace WorldRender.Graphics.Shaders
{
    /// <summary>
    /// Represents a group of compiled shaders that belong together.
    /// </summary>
    public sealed class CompiledShader
    {
        private PixelShader pixelShader;
        private VertexShader vertexShader;

        /// <summary>
        /// Gets the compiled pixel shader.
        /// </summary>
        public PixelShader PixelShader
        {
            get
            {
                return pixelShader;
            }
        }

        /// <summary>
        /// Gets the compiled vertex shader.
        /// </summary>
        public VertexShader VertexShader
        {
            get
            {
                return vertexShader;
            }
        }

        public CompiledShader(VertexShader vertexShader, PixelShader pixelShader)
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
    }
}

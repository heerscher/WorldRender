using System;
using System.Collections.Generic;

namespace WorldRender.Graphics
{
    /// <summary>
    /// A sortable batch of render calls.
    /// </summary>
    public class RenderCommand : IComparable<RenderCommand>
    {
        private UInt64 sortKey;
        private ConstantBuffer<Shaders.VertexConstantBuffer> vertexConstantBuffer;

        public Mesh Mesh { get; set; }
        public MeshGroup MeshGroup { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public RenderTarget RenderTarget { get; set; }
        public Shaders.CompiledShader Shader { get; set; }
        public Texture2d Texture { get; set; }
        public Shaders.VertexConstantBuffer VertexConstants;

        public RenderCommand()
        {
            UpdateSortKey();
        }

        public RenderCommand(Resources.Cache cache)
        {
#if ASSERT
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
#endif
            
            RasterizerState = cache.Get<RasterizerState>("default");
            RenderTarget = cache.Get<RenderTarget>("screen");
            vertexConstantBuffer = cache.VertexConstantBuffer;

            UpdateSortKey();
        }

        public void UpdateSortKey()
        {
            // Calculate sortkey
            sortKey = 0UL;
            sortKey += RenderTarget != null ? Convert.ToUInt64(RenderTarget.Id) << 60 : 0;
            sortKey += Shader != null && Shader.VertexShader != null ? Convert.ToUInt64(Shader.VertexShader.Id) << 56 : 0;
            sortKey += Shader != null && Shader.PixelShader != null ? Convert.ToUInt64(Shader.PixelShader.Id) << 52 : 0;
            sortKey += RasterizerState != null ? Convert.ToUInt64(RasterizerState.Id) << 48 : 0;
            sortKey += Texture != null ? Convert.ToUInt64(Texture.Id) << 44 : 0;
        }

        public void Render(Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            if (RenderTarget != null && Shader != null && Shader.VertexShader != null && Shader.PixelShader != null && RasterizerState != null && vertexConstantBuffer != null)
            {
                RenderTarget.Render(device.Context);
                Shader.VertexShader.Render(device.Context);

                vertexConstantBuffer.Write(ref VertexConstants);
                vertexConstantBuffer.UpdateVertexShader(device.Context, 0);

                Shader.PixelShader.Render(device.Context);

                if (Texture != null)
                {
                    Texture.Render(device.Context);
                }

                RasterizerState.Render(device.Context);

                if (MeshGroup != null)
                {
                    foreach (var mesh in MeshGroup.Meshes)
                    {
                        RenderMesh(device, mesh);
                    }
                }

                RenderMesh(device, Mesh);
            }
        }

        private void RenderMesh(Device device, Mesh mesh)
        {
            if (Mesh != null && Mesh.VertexBuffer != null)
            {
                Mesh.VertexBuffer.Render(device.Context);

                if (Mesh.IndexBuffer == null)
                {
                    device.Context.Draw(Mesh.VertexBuffer.Count, 0);
                }
                else
                {
                    Mesh.IndexBuffer.Render(device.Context);

                    device.Context.DrawIndexed(Mesh.IndexBuffer.Count, 0, 0);
                }
            }
        }

        public int CompareTo(RenderCommand other)
        {
            return Convert.ToInt32(sortKey - other.sortKey);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRender.Resources.Loaders
{
    public class MeshLoader : Loader
    {
        private Graphics.Device device;

        public MeshLoader(Graphics.Device device)
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
            using (var assimpContext = new Assimp.AssimpContext())
            {
                var scene = assimpContext.ImportFile(identifier);

                foreach (var mesh in scene.Meshes)
                {
                    var bytesPerVertex = 12;

                    if (mesh.HasNormals)
                    {
                        bytesPerVertex += 12;
                    }

                    var vertexCount = mesh.VertexCount;
                    var meshSizeInBytes = vertexCount * bytesPerVertex;

                    using (var stream = new SlimDX.DataStream(meshSizeInBytes, true, true))
                    {
                        for (var i = 0; i < vertexCount; ++i)
                        {
                            stream.Write(mesh.Vertices[i]);

                            if (mesh.HasNormals)
                            {
                                stream.Write(mesh.Normals[i]);
                            }

                            //dataStream.Write(sourceMesh.TextureCoordinateChannels[0][i]);
                        }

                        var vertexBuffer = new Graphics.VertexBuffer(device.Handle, stream, bytesPerVertex, vertexCount, SlimDX.Direct3D11.PrimitiveTopology.TriangleList);
                        var indices = mesh.GetIndices();

                        if (indices != null && indices.Count() > 0)
                        {
                            var indexBuffer = new Graphics.IndexBuffer(device.Handle, indices);

                            return new Graphics.Mesh(vertexBuffer, indexBuffer);
                        }

                        return new Graphics.Mesh(vertexBuffer);
                    }
                }

                throw new KeyNotFoundException("Failed to load mesh: " + identifier);
            }
        }
    }
}

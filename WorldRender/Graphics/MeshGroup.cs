using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class MeshGroup : IDisposable
    {
        private IEnumerable<Mesh> meshes;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return meshes;
            }
        }

        public MeshGroup(IEnumerable<Mesh> meshes)
        {
#if ASSERT
            if (meshes == null)
            {
                throw new ArgumentNullException("meshes");
            }
#endif

            this.meshes = meshes;
        }

        public void Dispose()
        {
            if (meshes != null)
            {
                foreach (var mesh in meshes)
                {
                    mesh.Dispose();
                }
            }

            meshes = null;
        }
    }
}

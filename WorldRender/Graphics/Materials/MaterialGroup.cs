using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRender.Graphics.Materials
{
    public class MaterialGroup : IDisposable
    {
        private IEnumerable<Material> materials;

        public int Count
        {
            get
            {
                return materials.Count();
            }
        }

        public IEnumerable<Material> Materials
        {
            get
            {
                return materials;
            }
        }

        public MaterialGroup(IEnumerable<Material> materials)
        {
#if ASSERT
            if (materials == null)
            {
                throw new ArgumentNullException("materials");
            }
#endif

            this.materials = materials;
        }

        public void Dispose()
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    material.Dispose();
                }

                materials = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics.Materials
{
    public class Material : IDisposable
    {
        private MaterialDescriptor materialDescriptor;

        public string Name
        {
            get
            {
                return materialDescriptor.Name;
            }
        }

        public Material(MaterialDescriptor materialDescriptor)
        {
#if ASSERT
            if (materialDescriptor == null)
            {
                throw new ArgumentNullException("materialDescriptor");
            }
#endif

            this.materialDescriptor = materialDescriptor;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}

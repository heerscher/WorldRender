using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities.Components
{
    public class TransformComponent : Component
    {
        public SlimDX.Matrix Matrix { get; set; }

        public TransformComponent(Entity entity)
            : base(entity)
        {
            Matrix = SlimDX.Matrix.Identity;
        }
    }
}

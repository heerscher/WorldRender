using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities.Components
{
    public class RenderComponent : Component
    {
        public Graphics.RenderCommand RenderCommand { get; set; }

        public RenderComponent(Entity entity)
            : base(entity)
        {
        }
    }
}

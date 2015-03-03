using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities.Components
{
    public abstract class Component
    {
        private Entity entity;

        protected Entity Entity
        {
            get
            {
                return entity;
            }
        }

        public Component(Entity entity)
        {
#if ASSERT
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
#endif

            this.entity = entity;
        }
    }
}

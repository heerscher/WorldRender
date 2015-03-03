using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities
{
    public class EntityCollection
    {
        private const int DefaultCapacity = 32;

        private List<Entity> entities;
        private List<Graphics.RenderCommand> renderCommands;

        public EntityCollection()
            : this(DefaultCapacity)
        {
        }

        public EntityCollection(int capacity)
        {
#if ASSERT
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
#endif

            entities = new List<Entity>(capacity);
            renderCommands = new List<Graphics.RenderCommand>(capacity);
        }

        public Entity CreateEntity()
        {
            var result = new Entity();

            entities.Add(result);

            return result;
        }

        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : Components.Component
        {
            return entities.Where(e => e.HasComponent<TComponent>()).Select(e => e.GetComponent<TComponent>());
        }

        public IEnumerable<Graphics.RenderCommand> Render(ref SlimDX.Matrix view, ref SlimDX.Matrix projection)
        {
            renderCommands.Clear();

            Entities.Components.RenderComponent renderComponent = null;

            foreach (var entity in entities)
            {
                renderComponent = entity.GetComponent<Entities.Components.RenderComponent>();

                if (renderComponent != null)
                {
                    var renderCommand = renderComponent.Render(ref view, ref projection);

                    if (renderCommand != null)
                    {
                        renderCommands.Add(renderCommand);
                    }
                }
            }

            renderCommands.Sort();

            return renderCommands;
        }
    }
}

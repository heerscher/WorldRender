using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRender.Entities
{
    /// <summary>
    /// A collection of entities.
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EntityCollection
    {
        private const int DefaultCapacity = 1024;

        private List<Entity> entities;

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
        }

        /// <summary>
        /// Removes all entities from the collection.
        /// </summary>
        public void Clear()
        {
            entities.Clear();
        }

        /// <summary>
        /// Creates a new entity that is part of this collection.
        /// </summary>
        public Entity CreateEntity()
        {
            return CreateEntity<Entity>();
        }

        /// <summary>
        /// Creates a new entity that is part of this collection.
        /// </summary>
        public TEntity CreateEntity<TEntity>() where TEntity : Entity
        {
            var result = Activator.CreateInstance<TEntity>();

            entities.Add(result);

            return result;
        }

        /// <summary>
        /// Gets all components of a specific type from all entities.
        /// </summary>
        public IEnumerable<TComponent> GetComponents<TComponent>() where TComponent : Components.Component
        {
            return entities.Where(e => e.HasComponent<TComponent>())
                            .Select(e => e.GetComponent<TComponent>());
        }
    }
}

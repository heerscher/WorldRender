using System;
using System.Collections.Generic;

namespace WorldRender.Entities
{
    /// <summary>
    /// Represents an entity that is part of a scene graph.
    /// </summary>
    public class Entity
    {
        private readonly Dictionary<Type, Components.Component> components;
        private readonly Components.TransformComponent transformComponent;
        private readonly Graphics.UniqueId<Entity> uniqueId;

        /// <summary>
        /// Gets the entity's unique identifier.
        /// </summary>
        public int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        /// <summary>
        /// Gets the parent of this entity.
        /// </summary>
        public Entity Parent { get; set; }

        /// <summary>
        /// Gets the entity's transform component.
        /// All entities always have this component.
        /// </summary>
        public Components.TransformComponent Transform
        {
            get
            {
                return transformComponent;
            }
        }

        /// <summary>
        /// Creates a new entity containing only a transform component.
        /// </summary>
        public Entity()
        {
            components = new Dictionary<Type, Components.Component>(2);
            uniqueId = new Graphics.UniqueId<Entity>();

            // Always add a transform component to the entity
            transformComponent = AddComponent<Components.TransformComponent>();
        }

        /// <summary>
        /// Adds a component of a specific type from this entity.
        /// If the component was already added, returns the previously created instance of the component.
        /// </summary>
        /// <typeparam name="TComponent">The component type to grab.</typeparam>
        public TComponent AddComponent<TComponent>() where TComponent : Components.Component
        {
            var type = typeof(TComponent);

            if (components.ContainsKey(type))
            {
                return (TComponent)components[type];
            }
            else
            {
                var result = (TComponent)Activator.CreateInstance(type, new object[] { this });

                components.Add(type, result);

                return result;
            }
        }

        /// <summary>
        /// Gets a component of a specific type from this entity.
        /// Returns null when the component is not part of the entity.
        /// </summary>
        /// <typeparam name="TComponent">The component type to grab.</typeparam>
        public TComponent GetComponent<TComponent>() where TComponent : Components.Component
        {
            var type = typeof(TComponent);

            if (components.ContainsKey(type))
            {
                return (TComponent)components[type];
            }

            return default(TComponent);
        }

        /// <summary>
        /// Determines whether the entity contains the specified component.
        /// </summary>
        /// <typeparam name="TComponent">The component type to check for.</typeparam>
        public bool HasComponent<TComponent>() where TComponent : Components.Component
        {
            return components.ContainsKey(typeof(TComponent));
        }

        /// <summary>
        /// Determines whether the entity contains the specified component.
        /// </summary>
        /// <param name="type">The component type to check for.</param>
        public bool HasComponent(Type type)
        {
#if ASSERT
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
#endif

            return components.ContainsKey(type);
        }
    }
}

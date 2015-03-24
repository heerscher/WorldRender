using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Entities
{
    public class Entity
    {
        // To prevent continuously creating type arrays
        private static readonly Type[] ConstructorTypes = new Type[] { typeof(Entity) };

        private Dictionary<Type, Components.Component> components;
        private Components.TransformComponent transformComponent;
        private Graphics.UniqueId<Entity> uniqueId;

        public int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        public Entity Parent { get; set; }

        public Components.TransformComponent Transform
        {
            get
            {
                return transformComponent;
            }
        }

        public Entity()
        {
            components = new Dictionary<Type, Components.Component>(2);
            uniqueId = new Graphics.UniqueId<Entity>();

            // Always add a transform component to the entity
            transformComponent = AddComponent<Components.TransformComponent>();
        }

        public T AddComponent<T>() where T : Components.Component
        {
            var type = typeof(T);

            if (components.ContainsKey(type))
            {
                return (T)components[type];
            }
            else
            {
                var constructor = type.GetConstructor(ConstructorTypes);
                var result = (T)constructor.Invoke(new object[] { this });

                components.Add(type, result);

                return result;
            }
        }

        public T GetComponent<T>() where T : Components.Component
        {
            var type = typeof(T);

            if (components.ContainsKey(type))
            {
                return (T)components[type];
            }

            return default(T);
        }

        public bool HasComponent<T>() where T : Components.Component
        {
            return components.ContainsKey(typeof(T));
        }

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

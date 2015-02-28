using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources
{
    public class Cache
    {
        private Dictionary<Type, Loader> loaders;
        private Dictionary<string, IDisposable> resources;

        public Cache()
        {
            loaders = new Dictionary<Type, Loader>();
            resources = new Dictionary<string, IDisposable>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Unloads all resources from memory and clears the cache.
        /// </summary>
        public void Unload()
        {
            foreach (var resource in resources.Values)
            {
                resource.Dispose();
            }

            resources.Clear();
        }

        public void RegisterLoader(Loader resourceLoader)
        {
#if ASSERT
            if (resourceLoader == null)
            {
                throw new ArgumentNullException("resourceLoader");
            }
#endif

            foreach (var resourceType in resourceLoader.SupportedTypes)
            {
                if (!loaders.ContainsKey(resourceType))
                {
                    loaders.Add(resourceType, resourceLoader);
                }
            }
        }

        public TResource Get<TResource>(string identifier) where TResource : IDisposable
        {
#if ASSERT
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            if (identifier.Length == 0)
            {
                throw new ArgumentOutOfRangeException("identifier");
            }
#endif

            if (resources.ContainsKey(identifier))
            {
                // Resource already loaded, return it
                return (TResource)resources[identifier];
            }
            else
            {
                // Attempt to load the resource
                var resourceType = typeof(TResource);
                var resourceKey = string.Concat(resourceType.FullName, "/", identifier);

                if (!loaders.ContainsKey(resourceType))
                {
                    throw new NotSupportedException("No resource loader registered for type " + resourceType.FullName);
                }

                var loader = loaders[resourceType];
                var resource = loader.Load(resourceType, identifier);

                resources.Add(resourceKey, resource);

                return (TResource)resource;
            }
        }
    }
}

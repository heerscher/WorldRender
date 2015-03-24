using System;
using System.Collections.Generic;

namespace WorldRender.Resources
{
    public class Cache : IDisposable
    {
        private Dictionary<Type, Loaders.BaseLoader> loaders;
        private Dictionary<string, IDisposable> resources;
        private Graphics.ConstantBuffer<Graphics.Shaders.VertexConstantBuffer> vertexConstantBuffer;

        public Graphics.ConstantBuffer<Graphics.Shaders.VertexConstantBuffer> VertexConstantBuffer
        {
            get
            {
                return vertexConstantBuffer;
            }
        }

        public Cache(Graphics.Device device)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            loaders = new Dictionary<Type, Loaders.BaseLoader>();
            resources = new Dictionary<string, IDisposable>(StringComparer.OrdinalIgnoreCase);
            vertexConstantBuffer = new Graphics.ConstantBuffer<Graphics.Shaders.VertexConstantBuffer>(device.Handle);

            // Map resource loaders to specific types (these are the default loaders for the engine)
            RegisterLoader(new Resources.Loaders.TextureLoader(device));
            RegisterLoader(new Resources.Loaders.MeshLoader(device));
            RegisterLoader(new Resources.Loaders.MaterialLoader());
            RegisterLoader(new Resources.Loaders.VertexShaderLoader(device));
            RegisterLoader(new Resources.Loaders.PixelShaderLoader(device));
            RegisterLoader(new Resources.Loaders.RasterizerStateLoader(device));
            RegisterLoader(new Resources.Loaders.RenderTargetLoader(device));
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

        public void RegisterLoader(Loaders.BaseLoader resourceLoader)
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

            var resourceType = typeof(TResource);
            var resourceKey = string.Concat(resourceType.FullName, "/", identifier);

            if (resources.ContainsKey(resourceKey))
            {
                // Resource already loaded, return it
                return (TResource)resources[resourceKey];
            }
            else
            {
                // Attempt to load the resource
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

        public void Dispose()
        {
            Unload();

            loaders.Clear();

            vertexConstantBuffer.Dispose();
        }
    }
}

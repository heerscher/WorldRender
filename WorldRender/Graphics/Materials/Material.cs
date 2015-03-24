﻿using System;
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

        public Graphics.RasterizerState GetRasterizerState(Resources.Cache cache)
        {
#if ASSERT
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
#endif

            if (materialDescriptor.Wireframe)
            {
                return cache.Get<Graphics.RasterizerState>("back;wireframe");
            }
            else
            {
                return cache.Get<Graphics.RasterizerState>("default");
            }
        }

        public Graphics.Shaders.CompiledShader GetShader(Resources.Cache cache)
        {
#if ASSERT
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
#endif

            if (string.IsNullOrEmpty(materialDescriptor.ShaderIdentifier))
            {
                // TODO: compile dynamic shader based on material flags
                throw new NotSupportedException("Materials with dynamic shader are not supported yet.");
            }
            else
            {
                var vertexShader = cache.Get<Graphics.Shaders.VertexShader>(materialDescriptor.ShaderIdentifier);
                var pixelShader = cache.Get<Graphics.Shaders.PixelShader>(materialDescriptor.ShaderIdentifier);

                return new Shaders.CompiledShader(vertexShader, pixelShader);
            }
        }

        public void Dispose()
        {
            // Empty, IDisposable is just a requirement of the resource loading system, even if not needed.
        }
    }
}
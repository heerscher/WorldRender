﻿using System;

namespace WorldRender.Resources.Loaders
{
    public class TextureLoader : BaseLoader
    {
        private Graphics.Device device;

        public TextureLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.Texture)
            })
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
#endif

            this.device = device;
        }

        public override IDisposable Load(Type resourceType, string identifier)
        {
            return new Graphics.Texture(device.Handle, identifier);
        }
    }
}

using System;

namespace WorldRender.Resources.Loaders
{
    public class ConstantBufferLoader : BaseLoader
    {
        private Graphics.Device device;

        public ConstantBufferLoader(Graphics.Device device)
            : base(new Type[]
            {
                typeof(Graphics.ConstantBuffer),
                typeof(Graphics.ConstantBuffer<>)
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
            // Identifier maps to the type used as the generic type for the constant buffer


            return default(IDisposable);
        }
    }
}

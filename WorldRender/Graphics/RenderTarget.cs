using System;

namespace WorldRender.Graphics
{
    public class RenderTarget : IDisposable
    {
        private SlimDX.Direct3D11.RenderTargetView renderTargetView;
        private bool resourceOwner;
        private UniqueId<RenderTarget> uniqueId;

        internal int Id
        {
            get
            {
                return uniqueId.Id;
            }
        }

        internal RenderTarget(SlimDX.Direct3D11.RenderTargetView renderTargetView)
        {
#if ASSERT
            if (renderTargetView == null)
            {
                throw new ArgumentNullException("renderTargetView");
            }
#endif

            this.renderTargetView = renderTargetView;
            resourceOwner = false;
            uniqueId = new UniqueId<RenderTarget>();
        }

        internal RenderTarget(SlimDX.Direct3D11.Device device, SlimDX.Direct3D11.Resource resource)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }
#endif

            renderTargetView = new SlimDX.Direct3D11.RenderTargetView(device, resource);
            resourceOwner = true;
            uniqueId = new UniqueId<RenderTarget>();
        }

        internal void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }
#endif

            deviceContext.OutputMerger.SetTargets(renderTargetView);
        }

        public void Dispose()
        {
            if (resourceOwner && renderTargetView != null)
            {
                renderTargetView.Dispose();
            }

            renderTargetView = null;
        }
    }
}

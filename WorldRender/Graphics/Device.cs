using System;
using System.Collections.Generic;

namespace WorldRender.Graphics
{
    public class Device : IDisposable
    {
        private SlimDX.Color4 backgroundColor;
        private SlimDX.Direct3D11.Device device;
        private SlimDX.Direct3D11.DeviceContext deviceContext;
        private SlimDX.Windows.RenderForm form;
        private SlimDX.Direct3D11.RenderTargetView screenRenderTargetView;
        private SlimDX.DXGI.SwapChain swapChain;
        private SlimDX.DXGI.SwapChainDescription swapChainDescription;
        private SlimDX.Direct3D11.Viewport viewport;

        /// <summary>
        /// Gets the device context of the internal DirectX 11 device.
        /// </summary>
        internal SlimDX.Direct3D11.DeviceContext Context
        {
            get
            {
                return deviceContext;
            }
        }

        /// <summary>
        /// Gets the internal DirectX11 device.
        /// </summary>
        internal SlimDX.Direct3D11.Device Handle
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// Gets the Windows Form form that is rendered to by this device.
        /// </summary>
        public System.Windows.Forms.Form Form
        {
            get
            {
                return form;
            }
        }

        public Device()
        {
            // Default background color
            backgroundColor = new SlimDX.Color4(0.5f, 0.5f, 1.0f);

            form = new SlimDX.Windows.RenderForm()
            {
                Text = "Game",
                WindowState = System.Windows.Forms.FormWindowState.Maximized
            };

            swapChainDescription = new SlimDX.DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                Usage = SlimDX.DXGI.Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new SlimDX.DXGI.ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new SlimDX.Rational(60, 1), SlimDX.DXGI.Format.R8G8B8A8_UNorm),
                SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                Flags = SlimDX.DXGI.SwapChainFlags.AllowModeSwitch,
                SwapEffect = SlimDX.DXGI.SwapEffect.Discard
            };

            var result = SlimDX.Direct3D11.Device.CreateWithSwapChain(SlimDX.Direct3D11.DriverType.Hardware, SlimDX.Direct3D11.DeviceCreationFlags.SingleThreaded, swapChainDescription, out device, out swapChain);

            if (result.IsFailure)
            {
                throw new ApplicationException("Failed to initialize DirectX 11.");
            }

            deviceContext = device.ImmediateContext;

            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<SlimDX.Direct3D11.Texture2D>(swapChain, 0))
            {
                screenRenderTargetView = new SlimDX.Direct3D11.RenderTargetView(device, resource);
            }

            using (var factory = swapChain.GetParent<SlimDX.DXGI.Factory>())
            {
                factory.SetWindowAssociation(form.Handle, SlimDX.DXGI.WindowAssociationFlags.IgnoreAltEnter);
            }

            viewport = new SlimDX.Direct3D11.Viewport(0.0f, 0.0f, Convert.ToSingle(form.ClientSize.Width), Convert.ToSingle(form.ClientSize.Height));

            deviceContext.Rasterizer.SetViewports(viewport);

            form.KeyDown += (sender, e) =>
            {
                if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
                }
            };

            form.UserResized += (sender, e) =>
            {
                swapChain.ResizeBuffers(2, 0, 0, SlimDX.DXGI.Format.R8G8B8A8_UNorm, SlimDX.DXGI.SwapChainFlags.AllowModeSwitch);

                using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<SlimDX.Direct3D11.Texture2D>(swapChain, 0))
                {
                    screenRenderTargetView.Dispose();
                    screenRenderTargetView = new SlimDX.Direct3D11.RenderTargetView(device, resource);
                }

                viewport = new SlimDX.Direct3D11.Viewport(0.0f, 0.0f, Convert.ToSingle(form.ClientSize.Width), Convert.ToSingle(form.ClientSize.Height));

                deviceContext.Rasterizer.SetViewports(viewport);
            };
        }

        public RenderTarget CreateScreenRenderTarget()
        {
            return new RenderTarget(screenRenderTargetView);
        }

        public void Render(IEnumerable<RenderCommand> renderCommands)
        {
            deviceContext.ClearRenderTargetView(screenRenderTargetView, backgroundColor);

            foreach (var renderCommand in renderCommands)
            {
                renderCommand.Render(this);
            }

            swapChain.Present(0, SlimDX.DXGI.PresentFlags.None);
        }

        public void Dispose()
        {
            if (screenRenderTargetView != null)
            {
                screenRenderTargetView.Dispose();
                screenRenderTargetView = null;
            }

            if (swapChain != null)
            {
                swapChain.Dispose();
                swapChain = null;
            }

            if (deviceContext != null)
            {
                deviceContext.Dispose();
                deviceContext = null;
            }

            if (device != null)
            {
                device.Dispose();
                device = null;
            }

            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }
    }
}

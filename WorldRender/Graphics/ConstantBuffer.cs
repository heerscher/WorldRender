using System;

namespace WorldRender.Graphics
{
    public sealed class ConstantBuffer<T> : ConstantBuffer where T : struct
    {
        internal ConstantBuffer(SlimDX.Direct3D11.Device device)
            : base(device, typeof(T))
        {
        }

        public void Change(ref T data)
        {
            stream.Position = 0;
            stream.Write(data);
            stream.Position = 0;
        }
    }

    public abstract class ConstantBuffer : IDisposable
    {
        private SlimDX.Direct3D11.Buffer constantBuffer;
        private bool resourceOwner;
        private int sizeInBytes;
        protected SlimDX.DataStream stream;

        internal ConstantBuffer(SlimDX.Direct3D11.Device device, Type dataType)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (dataType == null)
            {
                throw new ArgumentNullException("dataType");
            }

            if (!dataType.IsLayoutSequential || !dataType.IsValueType)
            {
                throw new NotSupportedException("dataType must be a struct with a sequential layout => [StructLayout(LayoutKind.Sequential, Pack = 16)].");
            }
#endif

            resourceOwner = true;
            sizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(dataType);
            stream = new SlimDX.DataStream(sizeInBytes, true, true);
            constantBuffer = new SlimDX.Direct3D11.Buffer(device, stream, new SlimDX.Direct3D11.BufferDescription
            {
                Usage = SlimDX.Direct3D11.ResourceUsage.Default,
                SizeInBytes = sizeInBytes,
                BindFlags = SlimDX.Direct3D11.BindFlags.ConstantBuffer
            });
        }

        internal void UpdatePixelShader(SlimDX.Direct3D11.DeviceContext deviceContext, int slot)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }

            if (slot < 0)
            {
                throw new ArgumentOutOfRangeException("slot");
            }
#endif

            deviceContext.UpdateSubresource(new SlimDX.DataBox(0, 0, stream), constantBuffer, 0);
            deviceContext.PixelShader.SetConstantBuffer(constantBuffer, slot);
        }

        internal void UpdateVertexShader(SlimDX.Direct3D11.DeviceContext deviceContext, int slot)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }

            if (slot < 0)
            {
                throw new ArgumentOutOfRangeException("slot");
            }
#endif

            deviceContext.UpdateSubresource(new SlimDX.DataBox(0, 0, stream), constantBuffer, 0);
            deviceContext.VertexShader.SetConstantBuffer(constantBuffer, slot);
        }

        public void Dispose()
        {
            if (resourceOwner)
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                if (constantBuffer != null)
                {
                    constantBuffer.Dispose();
                }
            }

            constantBuffer = null;
            stream = null;
        }
    }
}

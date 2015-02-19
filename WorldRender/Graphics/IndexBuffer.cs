using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class IndexBuffer : IDisposable
    {
        private SlimDX.Direct3D11.Buffer indexBuffer;
        private int indexCount;
        private bool resourceOwner;

        /// <summary>
        /// Gets the number of indices stored inside this buffer.
        /// </summary>
        public int Count
        {
            get
            {
                return indexCount;
            }
        }

        internal IndexBuffer(SlimDX.Direct3D11.Device device, IEnumerable<int> indices)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (indices == null)
            {
                throw new ArgumentNullException("indices");
            }

            if (indices.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("indices");
            }
#endif

            indexCount = indices.Count();
            resourceOwner = true;

            using (var dataStream = new SlimDX.DataStream(indexCount * 2, true, true))
            {
                foreach (var index in indices)
                {
                    dataStream.Write(Convert.ToUInt16(index));
                }

                dataStream.Position = 0;

                indexBuffer = new SlimDX.Direct3D11.Buffer(device, dataStream, 2 * indexCount, SlimDX.Direct3D11.ResourceUsage.Default, SlimDX.Direct3D11.BindFlags.IndexBuffer, SlimDX.Direct3D11.CpuAccessFlags.None, SlimDX.Direct3D11.ResourceOptionFlags.None, 0);
            }           
        }

        internal IndexBuffer(SlimDX.Direct3D11.Buffer indexBuffer, int indexCount)
        {
#if ASSERT
            if (indexBuffer == null)
            {
                throw new ArgumentNullException("indexBuffer");
            }
            
            if (indexCount <= 0)
            {
                throw new ArgumentNullException("indexCount");
            }
#endif

            this.indexBuffer = indexBuffer;
            this.indexCount = indexCount;
            resourceOwner = false;
        }

        internal void Render(SlimDX.Direct3D11.DeviceContext deviceContext)
        {
#if ASSERT
            if (deviceContext == null)
            {
                throw new ArgumentNullException("deviceContext");
            }
#endif

            deviceContext.InputAssembler.SetIndexBuffer(indexBuffer, SlimDX.DXGI.Format.R16_UNorm, 0);
        }

        public void Dispose()
        {
            if (resourceOwner)
            {
                if (indexBuffer != null)
                {
                    indexBuffer.Dispose();
                }
            }

            indexBuffer = null;
        }
    }
}


namespace WorldRender.Graphics.Shaders
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 16)]
    public struct VertexConstantBuffer
    {
        public SlimDX.Matrix World;
        public SlimDX.Matrix View;
        public SlimDX.Matrix Projection;
    }
}

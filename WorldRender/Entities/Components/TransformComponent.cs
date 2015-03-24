
namespace WorldRender.Entities.Components
{
    public class TransformComponent : Component
    {
        private SlimDX.Matrix matrix;

        public SlimDX.Matrix Matrix
        {
            get
            {
                return matrix;
            }
            set
            {
                matrix = value;
            }
        }

        public TransformComponent(Entity entity)
            : base(entity)
        {
            matrix = SlimDX.Matrix.Identity;
        }

        public void Translate(SlimDX.Vector3 translation)
        {
            matrix[0, 3] += translation.X;
            matrix[1, 3] += translation.Y;
            matrix[2, 3] += translation.Z;
        }
    }
}

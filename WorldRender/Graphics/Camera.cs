
namespace WorldRender.Graphics
{
    public class Camera
    {
        public SlimDX.Vector3 Position;
        private SlimDX.Vector3 right;
        private SlimDX.Vector3 forward;
        private SlimDX.Vector3 up;

        private float speed;

        public SlimDX.Matrix View;

        public bool MovingForwards { get; set; }
        public bool MovingBackwards { get; set; }
        public bool StrafingLeft { get; set; }
        public bool StrafingRight { get; set; }

        public float Pitch { get; set; }
        public float Angle { get; set; }

        public Camera()
        {
            Position = new SlimDX.Vector3(0.0f, 0.0f, -5.0f);
            right = new SlimDX.Vector3(1.0f, 0.0f, 0.0f);
            up = new SlimDX.Vector3(0.0f, 1.0f, 0.0f);
            forward = new SlimDX.Vector3(0.0f, 0.0f, 1.0f);
            speed = 2.0f;
            View = SlimDX.Matrix.Identity;
        }

        public void Update(float deltaTime)
        {
            // Find the net direction the camera is traveling in (since the
            // camera could be running and strafing).
            var direction = SlimDX.Vector3.Zero;

            if (MovingForwards)
            {
                direction += forward;
            }

            if (MovingBackwards)
            {
                direction -= forward;
            }

            if (StrafingLeft)
            {
                direction -= right;
            }

            if (StrafingRight)
            {
                direction += right;
            }

            direction.Normalize();

            // Move at mSpeed along net direction
            Position += direction * speed * deltaTime;

            //// We rotate at a fixed speed
            //float pitch = Convert.ToSingle(input.MouseDeltaY()) / sensitivity;
            //float angle = Convert.ToSingle(input.MouseDeltaX()) / sensitivity;

            // Rotate camera's look and up vectors around the camera's right vector
            var rotation = SlimDX.Matrix.RotationAxis(right, Pitch);

            forward = SlimDX.Vector3.TransformCoordinate(forward, rotation);
            up = SlimDX.Vector3.TransformCoordinate(up, rotation);

            // Rotate camera axes about the world's y-axis
            rotation = SlimDX.Matrix.RotationY(Angle);

            right = SlimDX.Vector3.TransformCoordinate(right, rotation);
            up = SlimDX.Vector3.TransformCoordinate(up, rotation);
            forward = SlimDX.Vector3.TransformCoordinate(forward, rotation);

            // Rebuild the view matrix to reflect changes
            BuildViewMatrix();
        }

        private void BuildViewMatrix()
        {
            // Keep camera's axes orthogonal to each other and of unit length
            forward.Normalize();

            up = SlimDX.Vector3.Cross(forward, right);
            up.Normalize();

            right = SlimDX.Vector3.Cross(up, forward);
            right.Normalize();

            // Fill in the view matrix entries
            View[0, 0] = right.X;
            View[1, 0] = right.Y;
            View[2, 0] = right.Z;
            View[3, 0] = -SlimDX.Vector3.Dot(Position, right);

            View[0, 1] = up.X;
            View[1, 1] = up.Y;
            View[2, 1] = up.Z;
            View[3, 1] = -SlimDX.Vector3.Dot(Position, up);

            View[0, 2] = forward.X;
            View[1, 2] = forward.Y;
            View[2, 2] = forward.Z;
            View[3, 2] = -SlimDX.Vector3.Dot(Position, forward);

            View[0, 3] = 0.0f;
            View[1, 3] = 0.0f;
            View[2, 3] = 0.0f;
            View[3, 3] = 1.0f;
        }
    }
}

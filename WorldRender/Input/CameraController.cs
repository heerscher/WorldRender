using System;
using System.Windows.Forms;

namespace WorldRender.Input
{
    public class CameraController
    {
        private Graphics.Camera camera;
        private IState inputState;
        private Command inputMoveForward;
        private Command inputMoveBackward;
        private Command inputStrafeLeft;
        private Command inputStrafeRight;
        private float mouseSensitivity;

        public CameraController(IState inputState, Graphics.Camera camera)
        {
            if (inputState == null)
            {
                throw new ArgumentNullException("inputState");
            }

            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            this.inputState = inputState;
            inputMoveForward = inputState.Register("moveForward");
            inputMoveBackward = inputState.Register("moveBackward");
            inputStrafeLeft = inputState.Register("strafeLeft");
            inputStrafeRight = inputState.Register("strafeRight");
            this.camera = camera;
            mouseSensitivity = 500.0f;
        }

        public void Update(float deltaTime)
        {
            camera.MovingForwards = inputMoveForward.Down;
            camera.MovingBackwards = inputMoveBackward.Down;
            camera.StrafingLeft = inputStrafeLeft.Down;
            camera.StrafingRight = inputStrafeRight.Down;
            camera.Pitch = Convert.ToSingle(inputState.MouseDeltaY()) / mouseSensitivity;
            camera.Angle = Convert.ToSingle(inputState.MouseDeltaX()) / mouseSensitivity;
            camera.Update(deltaTime);
        }
    }
}

using System;

namespace WorldRender.Input
{
    public class CameraController
    {
		private Graphics.Camera camera;
		private IState inputState;
		
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
			this.camera = camera;
		}
		
		public void Update(float deltaTime)
		{
			camera.MovingForwards = inputState.IsKeyDown(Keys.W);
			camera.MovingBackwards = inputState.IsKeyDown(Keys.S);
			camera.StrafingLeft = inputState.IsKeyDown(Keys.A);
			camera.StrafingRight = inputState.IsKeyDown(Keys.D);
			camera.Pitch = Convert.ToSingle(inputState.MouseDeltaY()) / 500.0f;
			camera.Angle = Convert.ToSingle(inputState.MouseDeltaX()) / 500.0f;
			
			camera.Update(deltaTime);
		}
    }
}

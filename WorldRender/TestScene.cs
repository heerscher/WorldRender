using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender
{
    public class TestScene : Scene.Scene
    {
        private Input.CameraController cameraController;

        public TestScene(GameInstance instance)
            : base(instance)
        {
            cameraController = new Input.CameraController(Input, Camera);
            CreateEntity(Cache.Get<Graphics.Mesh>("simplecube.DAE"), Cache.Get<Graphics.Materials.Material>("default"));
        }

        public override void OnUpdate(float deltaTime)
        {
            cameraController.Update(deltaTime);
        }
    }
}

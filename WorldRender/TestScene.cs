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
            cameraController = new Input.CameraController(InputState, Camera);
            CreateTestEntity(instance, this);
        }

        public override void OnUpdate(float deltaTime)
        {
            cameraController.Update(deltaTime);
        }

        public Entities.Entity CreateTestEntity(GameInstance instance, Scene.Scene scene)
        {
            var entity = scene.Entities.CreateEntity();
            var renderComponent = entity.AddComponent<Entities.Components.RenderComponent>();
            var material = instance.ResourceCache.Get<Graphics.Materials.Material>("default");

            renderComponent.RenderCommand = new Graphics.RenderCommand(instance.ResourceCache)
            {
                Mesh = instance.ResourceCache.Get<Graphics.Mesh>("simplecube.DAE"),
                RasterizerState = material.GetRasterizerState(instance.ResourceCache),
                Shader = material.GetShader(instance.ResourceCache),
                Texture = instance.ResourceCache.Get<Graphics.Texture2d>("uv_map_reference.jpg")
            };

            return entity;
        }
    }
}

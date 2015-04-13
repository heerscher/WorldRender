using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Scene
{
    public class Scene
    {
        private Graphics.Device device;
        private Entities.EntityCollection entities;
        private Input.IState inputState;
        private SlimDX.Matrix projection;
        private List<Graphics.RenderCommand> renderCommands;
        private Resources.Cache resourceCache;

        public Graphics.Camera Camera { get; set; }

        public Entities.EntityCollection Entities
        {
            get
            {
                return entities;
            }
        }

        public Input.IState InputState
        {
            get
            {
                return inputState;
            }
        }

        public Scene(GameInstance instance)
            : this(instance.Device, instance.InputState, instance.ResourceCache)
        {
        }

        internal Scene(Graphics.Device device, Input.IState inputState, Resources.Cache resourceCache)
        {
#if ASSERT
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            if (inputState == null)
            {
                throw new ArgumentNullException("inputState");
            }

            if (resourceCache == null)
            {
                throw new ArgumentNullException("resourceCache");
            }
#endif

            Camera = new Graphics.Camera();
            this.device = device;
            entities = new Entities.EntityCollection();
            this.inputState = inputState;
            renderCommands = new List<Graphics.RenderCommand>(64);
            this.resourceCache = resourceCache;

            // Calculate default projection matrix
            var fieldOfView = Convert.ToSingle(Math.PI) / 4.0f;
            var aspectRatio = Convert.ToSingle(device.Form.Width) / Convert.ToSingle(device.Form.Height);
            var near = 0.1f;
            var far = 1000.0f;

            SetProjection(fieldOfView, aspectRatio, near, far);
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// Sets the projection matrix using specific values.
        /// </summary>
        public void SetProjection(float fieldOfView, float aspectRatio, float near, float far)
        {
            projection = SlimDX.Matrix.Transpose(SlimDX.Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, near, far));
        }

        internal void Render()
        {
            // Create the view matrix from camera parameters
            var view = SlimDX.Matrix.Transpose(Camera.View);

            // Convert renderable entities into render commands and sort them
            renderCommands.Clear();
            renderCommands.AddRange(entities.GetComponents<Entities.Components.RenderComponent>().Select(r => UpdateRenderCommandConstants(r, ref view, ref projection)));
            renderCommands.Sort();

            device.Render(renderCommands);
        }

        private Graphics.RenderCommand UpdateRenderCommandConstants(Entities.Components.RenderComponent renderComponent, ref SlimDX.Matrix view, ref SlimDX.Matrix projection)
        {
            renderComponent.RenderCommand.VertexConstants.World = renderComponent.Entity.Transform.Matrix;
            renderComponent.RenderCommand.VertexConstants.View = view;
            renderComponent.RenderCommand.VertexConstants.Projection = projection;

            return renderComponent.RenderCommand;
        }
    }
}

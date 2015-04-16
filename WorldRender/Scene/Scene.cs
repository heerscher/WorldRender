using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRender.Scene
{
    /// <summary>
    /// Represents a scene or screen inside the game.
    /// Can be derived from, but can also run as a generic scene containing entities.
    /// </summary>
    public class Scene
    {
        private Entities.EntityCollection entities;
        private GameInstance gameInstance;
        private SlimDX.Matrix projection;
        private List<Graphics.RenderCommand> renderCommands;
        private Resources.Cache resourceCache;

        /// <summary>
        /// Gets or sets the camera associated with this scene.
        /// </summary>
        public Graphics.Camera Camera { get; set; }

        /// <summary>
        /// Gets the entities that are part of this scene.
        /// </summary>
        public Entities.EntityCollection Entities
        {
            get
            {
                return entities;
            }
        }

        /// <summary>
        /// Gets the state of the input devices.
        /// </summary>
        public Input.IState Input
        {
            get
            {
                return gameInstance.Input;
            }
        }

        /// <summary>
        /// Gets the scene's resource cache.
        /// </summary>
        public Resources.Cache Cache
        {
            get
            {
                return resourceCache;
            }
        }

        public Scene(GameInstance gameInstance)
            : this(gameInstance, gameInstance.Device.CreateResourceCache())
        {
        }

        public Scene(GameInstance gameInstance, Resources.Cache resourceCache)
        {
#if ASSERT
            if (gameInstance == null)
            {
                throw new ArgumentNullException("gameInstance");
            }

            if (resourceCache == null)
            {
                throw new ArgumentNullException("resourceCache");
            }
#endif

            Camera = new Graphics.Camera();
            entities = new Entities.EntityCollection();
            this.gameInstance = gameInstance;
            renderCommands = new List<Graphics.RenderCommand>(64);
            this.resourceCache = resourceCache;

            // Calculate default projection matrix
            var fieldOfView = Convert.ToSingle(Math.PI) / 4.0f;
            var aspectRatio = Convert.ToSingle(gameInstance.Device.Form.Width) / Convert.ToSingle(gameInstance.Device.Form.Height);
            var near = 0.1f;
            var far = 1000.0f;

            SetProjection(fieldOfView, aspectRatio, near, far);
        }

        public void GotoScene<TScene>() where TScene : Scene
        {
            var scene = (TScene)Activator.CreateInstance(typeof(TScene), new object[] { gameInstance });

            GotoScene(scene);
        }

        public void GotoScene(Scene scene)
        {
#if ASSERT
            if (scene == null)
            {
                throw new ArgumentNullException("scene");
            }
#endif

            // Clean up the previous scene
            entities.Clear();
            renderCommands.Clear();
            resourceCache.Dispose();

            gameInstance.Scene = scene;
        }

        public Entities.Entity CreateEntity()
        {
            return entities.CreateEntity<Entities.Entity>();
        }

        public Entities.Entity CreateEntity(Graphics.Mesh mesh, Graphics.Materials.Material material)
        {
            return CreateEntity<Entities.Entity>(mesh, material);
        }

        public TEntity CreateEntity<TEntity>() where TEntity : Entities.Entity
        {
            return entities.CreateEntity<TEntity>();
        }

        public TEntity CreateEntity<TEntity>(Graphics.Mesh mesh, Graphics.Materials.Material material) where TEntity : Entities.Entity
        {
            var entity = CreateEntity<TEntity>();
            var renderComponent = entity.AddComponent<Entities.Components.RenderComponent>();
            var renderCommand = material.CreateRenderCommand(resourceCache);

            renderCommand.Mesh = mesh;
            renderComponent.RenderCommand = renderCommand;

            return entity;
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

            gameInstance.Device.Render(renderCommands);
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

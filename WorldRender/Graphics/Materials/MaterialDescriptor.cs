
namespace WorldRender.Graphics.Materials
{
    /// <summary>
    /// Describes a specific material, mainly used for serialization to and from JSON files.
    /// </summary>
    public class MaterialDescriptor
    {
        /// <summary>
        /// Gets or sets the name of the material being described.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the shader used by the material being described.
        /// </summary>
        public string ShaderIdentifier { get; set; }

        /// <summary>
        /// Gets or sets whether the material being described is opaque or transparent.
        /// </summary>
        public bool Opaque { get; set; }

        /// <summary>
        /// Gets or sets whether the material being described has a glossy shine.
        /// </summary>
        public bool Specular { get; set; }

        /// <summary>
        /// Gets or sets whether the material being described renders a wireframe of the model.
        /// </summary>
        public bool Wireframe { get; set; }

        /// <summary>
        /// Gets or sets whether the material being described uses a normal map.
        /// </summary>
        public bool NormalMap { get; set; }
    }
}

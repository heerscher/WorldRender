using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources.Loaders
{
    public class MaterialLoader : BaseLoader
    {
        private Dictionary<string, Graphics.Materials.Material> materials;

        public MaterialLoader()
            : base(new Type[]
            {
                typeof(Graphics.Materials.Material)
            })
        {
        }

        public override IDisposable Load(Type resourceType, string identifier)
        {
            if (materials == null)
            {
                var materialFileContent = System.IO.File.ReadAllText("materials.json");
                var descriptors = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Graphics.Materials.MaterialDescriptor>>(materialFileContent);

                materials = new Dictionary<string, Graphics.Materials.Material>(descriptors.Count(), StringComparer.OrdinalIgnoreCase);

                foreach (var descriptor in descriptors)
                {
                    if (materials.ContainsKey(descriptor.Name))
                    {
                        throw new ArgumentException("Duplicate material name '" + descriptor.Name + "' used in material file.");
                    }

                    materials.Add(descriptor.Name, new Graphics.Materials.Material(descriptor));
                }
            }

            if (materials.ContainsKey(identifier))
            {
                return materials[identifier];
            }
            else
            {
                throw new KeyNotFoundException("Failed to load material: " + identifier);
            }
        }
    }
}

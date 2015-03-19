using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources.Loaders
{
    public class MaterialLoader : BaseLoader
    {
        public MaterialLoader()
            : base(new Type[]
            {
                typeof(Graphics.Materials.MaterialGroup)
            })
        {
        }

        public override IDisposable Load(Type resourceType, string identifier)
        {
            var materialFileContent = System.IO.File.ReadAllText(identifier);
            var descriptors = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Graphics.Materials.MaterialDescriptor>>(materialFileContent);
            var materials = new List<Graphics.Materials.Material>(descriptors.Count());

            foreach (var descriptor in descriptors)
            {
                materials.Add(new Graphics.Materials.Material(descriptor));
            }

            return new Graphics.Materials.MaterialGroup(materials);
        }
    }
}

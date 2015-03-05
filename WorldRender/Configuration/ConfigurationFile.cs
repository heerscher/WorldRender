using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WorldRender.Configuration
{
    public sealed class ConfigurationFile<TConfiguration>
    {
        private string filePath;

        public ConfigurationFile(string filePath)
        {
#if ASSERT
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (filePath.Length == 0)
            {
                throw new ArgumentOutOfRangeException("filePath");
            }
#endif

            this.filePath = filePath;
        }

        public TConfiguration Read()
        {
            if (System.IO.File.Exists(filePath))
            {
                var contents = System.IO.File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(contents))
                {
                    return JsonConvert.DeserializeObject<TConfiguration>(contents);
                }
            }

            return default(TConfiguration);
        }

        public void Write(TConfiguration configuration)
        {
#if ASSERT
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
#endif

            var contents = JsonConvert.SerializeObject(configuration);

            if (!string.IsNullOrEmpty(contents))
            {
                System.IO.File.WriteAllText(filePath, contents);
            }
        }
    }
}

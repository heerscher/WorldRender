using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRender.Resources.Loaders
{
    public abstract class BaseLoader
    {
        private IEnumerable<Type> supportedTypes;

        public IEnumerable<Type> SupportedTypes
        {
            get
            {
                return supportedTypes;
            }
        }

        public BaseLoader(IEnumerable<Type> supportedTypes)
        {
#if ASSERT
            if (supportedTypes == null)
            {
                throw new ArgumentNullException("supportedTypes");
            }

            if (supportedTypes.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("supportedTypes");
            }
#endif

            this.supportedTypes = supportedTypes;
        }

        public abstract IDisposable Load(Type resourceType, string identifier);
    }
}

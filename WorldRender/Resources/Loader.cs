using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Resources
{
    public abstract class Loader
    {
        public abstract IDisposable Load(Type resourceType, string identifier);
    }
}

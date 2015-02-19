using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    public class UniqueId<T>
    {
        private int id;
        private static int previousId = 0;
        private static object syncRoot = new object();

        public int Id
        {
            get
            {
                return id;
            }
        }

        public UniqueId()
        {
            lock (syncRoot)
            {
                id = ++previousId;
            }
        }
    }
}

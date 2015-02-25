
namespace WorldRender.Graphics
{
    /// <summary>
    /// Represents an unique ID per type.
    /// Each instance of this class increases the ID by one.
    /// This class is threadsafe.
    /// </summary>
    /// <typeparam name="T">The type for which to grab the next ID from.</typeparam>
    public sealed class UniqueId<T>
    {
        private int id;
        private static int previousId = 0;
        private static object syncRoot = new object();

        /// <summary>
        /// Gets the unique ID generated when this instance was created.
        /// </summary>
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
                if (previousId == int.MaxValue)
                {
                    id = 0;
                }
                else
                {
                    id = ++previousId;
                }
            }
        }
    }
}

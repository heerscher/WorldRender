
namespace WorldRender.Graphics
{
    /// <summary>
    /// Represents an unique ID per type.
    /// Each instance of this class increases the ID by one.
    /// This class is threadsafe.
    /// </summary>
    /// <typeparam name="TUnique">The type for which to grab the next ID from.</typeparam>
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class UniqueId<TUnique>
    {
        private readonly int id;
        private static int previousId = 0;
        private static object syncRoot = new object();

        /// <summary>
        /// Gets the unique identifier for this type, generated when this instance was created.
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
            // We need this part to be threadsafe to guarantee that there are no duplicate ids
            lock (syncRoot)
            {
                // Generate a new id for this instance
                id = ++previousId;

                // It is very unlikely that the previousId will ever overflow, but in case it happens we want the system to throw the usual exception
                // In case you have detected an overflow in the above line of code, you can resolve it by simply increasing the previousId and id bitsize (i.e. change the type from Int32 to Int64)
            }
        }
    }
}

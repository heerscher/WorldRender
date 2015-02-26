using System;

namespace WorldRender.Input
{
    public sealed class Command
    {
        private string name;

        public string Description { get; set; }
        public bool Down { get; set; }
        public bool Pressed { get; set; }
        public IBinding PrimaryBinding { get; set; }
        public IBinding SecondaryBinding { get; set; }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Command(string name)
        {
#if ASSERT
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentOutOfRangeException("name");
            }
#endif

            this.name = name;
        }
    }
}

using System.Windows.Forms;

namespace WorldRender.Input
{
    public class KeyBinding : IBinding
    {
        private Keys key;

        public Command Command { get; set; }

        public Keys Key
        {
            get
            {
                return key;
            }
        }

        public KeyBinding(Keys key)
        {
            this.key = key;
        }

        public bool IsPressed(IState inputState)
        {
            return inputState.IsKeyPressed(key);
        }

        public bool IsDown(IState inputState)
        {
            return inputState.IsKeyDown(key);
        }
    }
}

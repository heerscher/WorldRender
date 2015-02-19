using System.Collections.Generic;

namespace WorldRender.Input
{
    public class FormEventHandler : IState
    {
        private Dictionary<System.Windows.Forms.Keys, KeyState> keyState;
        private MouseState mouseState;

        public FormEventHandler(System.Windows.Forms.Form form)
        {
            keyState = new Dictionary<System.Windows.Forms.Keys, KeyState>();
            mouseState = new MouseState();

            form.KeyDown += KeyDownEvent;
            form.KeyUp += KeyUpEvent;
            form.MouseMove += MouseMoveEvent;
        }

        public bool IsKeyDown(System.Windows.Forms.Keys key)
        {
            return keyState.ContainsKey(key) && keyState[key].Down;
        }

        public bool IsKeyPressed(System.Windows.Forms.Keys key)
        {
            return keyState.ContainsKey(key) && keyState[key].Pressed;
        }

        public int MouseX()
        {
            return mouseState.X;
        }

        public int MouseY()
        {
            return mouseState.Y;
        }

        public int MouseDeltaX()
        {
            return mouseState.DeltaX;
        }

        public int MouseDeltaY()
        {
            return mouseState.DeltaY;
        }

        public void UpdateState()
        {
            // Update key pressed state
            foreach (var kvp in keyState)
            {
                var state = kvp.Value;

                if (state.PressedFlag)
                {
                    state.Pressed = true;
                    state.PressedFlag = false;
                }
            }

            // Update mouse delta
            mouseState.DeltaX = mouseState.LastX - mouseState.X;
            mouseState.DeltaY = mouseState.LastY - mouseState.Y;

            // Update current mouse coordinates
            mouseState.X = mouseState.LastX;
            mouseState.Y = mouseState.LastY;
        }

        private void MouseMoveEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseState.LastX = e.X;
            mouseState.LastY = e.Y;
        }

        private void KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (keyState.ContainsKey(e.KeyCode))
            {
                var state = keyState[e.KeyCode];

                state.Down = true;
                state.PressedFlag = true;
            }
            else
            {
                keyState.Add(e.KeyCode, new KeyState()
                {
                    Down = true,
                    PressedFlag = true
                });
            }
        }

        private void KeyUpEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var state = keyState[e.KeyCode];

            state.Down = false;
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace WorldRender.Input
{
    public class FormEventHandler : IState
    {
        private Form form;
        private Dictionary<System.Windows.Forms.Keys, KeyState> keyState;
        private MouseState mouseState;

        public FormEventHandler(System.Windows.Forms.Form form)
        {
#if ASSERT
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }
#endif

            this.form = form;
            keyState = new Dictionary<System.Windows.Forms.Keys, KeyState>();
            mouseState = new MouseState();

            form.KeyDown += KeyDownEvent;
            form.KeyUp += KeyUpEvent;
            
            // Put mouse in center of screen immediately, otherwise the first frame is way off
            Cursor.Position = new System.Drawing.Point(form.Width / 2, form.Height / 2);
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

            // Get mouse information
            var oldPosition = Cursor.Position;
            var newPosition = new System.Drawing.Point(form.Width / 2, form.Height / 2);

            // Update mouse delta
            mouseState.DeltaX = oldPosition.X - newPosition.X;
            mouseState.DeltaY = oldPosition.Y - newPosition.Y;

            // Update current mouse coordinates
            mouseState.X += mouseState.DeltaX;
            mouseState.Y += mouseState.DeltaY;

            // Put mouse in center of screen
            Cursor.Position = newPosition;
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
            if (keyState.ContainsKey(e.KeyCode))
            {
                var state = keyState[e.KeyCode];

                state.Down = false;
            }
        }
    }
}

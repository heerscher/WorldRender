using System;
using System.Collections.Generic;

namespace WorldRender.Input
{
    public class FormEventHandler : IState
    {
        private CommandCollection commandCollection;
        private System.Windows.Forms.Form form;
        private Dictionary<System.Windows.Forms.Keys, KeyState> keyState;
        private Dictionary<System.Windows.Forms.MouseButtons, MouseButtonState> mouseButtonState;
        private MouseState mouseState;

        public FormEventHandler(System.Windows.Forms.Form form)
        {
#if ASSERT
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }
#endif

            commandCollection = new CommandCollection();
            this.form = form;
            keyState = new Dictionary<System.Windows.Forms.Keys, KeyState>();
            mouseButtonState = new Dictionary<System.Windows.Forms.MouseButtons, MouseButtonState>();
            mouseState = new MouseState();

            form.KeyDown += KeyDownEvent;
            form.KeyUp += KeyUpEvent;
            form.MouseDown += MouseButtonDownEvent;
            form.MouseUp += MouseButtonUpEvent;

            // Put mouse in center of screen immediately, otherwise the first frame is way off
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(form.Width / 2, form.Height / 2);
        }

        public Command Register(string name)
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

            return commandCollection.Register(name);
        }

        public bool IsKeyDown(System.Windows.Forms.Keys key)
        {
            return keyState.ContainsKey(key) && keyState[key].Down;
        }

        public bool IsKeyPressed(System.Windows.Forms.Keys key)
        {
            return keyState.ContainsKey(key) && keyState[key].Pressed;
        }

        public bool IsMouseButtonDown(System.Windows.Forms.MouseButtons mouseButton)
        {
            return mouseButtonState.ContainsKey(mouseButton) && mouseButtonState[mouseButton].Down;
        }

        public bool IsMouseButtonPressed(System.Windows.Forms.MouseButtons mouseButton)
        {
            return mouseButtonState.ContainsKey(mouseButton) && mouseButtonState[mouseButton].Pressed;
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

            // Update mouse button state
            foreach (var mb in mouseButtonState)
            {
                var state = mb.Value;

                if (state.PressedFlag)
                {
                    state.Pressed = true;
                    state.PressedFlag = false;
                }
            }

            // Get mouse information
            var oldPosition = System.Windows.Forms.Cursor.Position;
            var newPosition = new System.Drawing.Point(form.Width / 2, form.Height / 2);

            // Update mouse delta
            mouseState.DeltaX = oldPosition.X - newPosition.X;
            mouseState.DeltaY = oldPosition.Y - newPosition.Y;

            // Update current mouse coordinates
            mouseState.X += mouseState.DeltaX;
            mouseState.Y += mouseState.DeltaY;

            // Put mouse in center of screen
            System.Windows.Forms.Cursor.Position = newPosition;

            // Update the binding information
            commandCollection.Update(this);
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

        private void MouseButtonDownEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseButtonState.ContainsKey(e.Button))
            {
                var state = mouseButtonState[e.Button];

                state.Down = true;
                state.PressedFlag = true;
            }
            else
            {
                mouseButtonState.Add(e.Button, new MouseButtonState()
                {
                    Down = true,
                    PressedFlag = true
                });
            }
        }

        private void MouseButtonUpEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseButtonState.ContainsKey(e.Button))
            {
                var state = mouseButtonState[e.Button];

                state.Down = false;
            }
        }
    }
}

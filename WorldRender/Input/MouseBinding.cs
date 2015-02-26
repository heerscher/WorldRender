using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WorldRender.Input
{
    public class MouseBinding : IBinding
    {
        private MouseButtons mouseButton;

        public MouseButtons MouseButton
        {
            get
            {
                return mouseButton;
            }
        }

        public MouseBinding(MouseButtons mouseButton)
        {
            this.mouseButton = mouseButton;
        }

        public bool IsPressed(IState inputState)
        {
            return inputState.IsMouseButtonPressed(mouseButton);
        }

        public bool IsDown(IState inputState)
        {
            return inputState.IsMouseButtonDown(mouseButton);
        }
    }
}

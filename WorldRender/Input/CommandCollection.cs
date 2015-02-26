using System;
using System.Collections.Generic;

namespace WorldRender.Input
{
    public sealed class CommandCollection : IEnumerable<Command>
    {
        private Dictionary<string, Command> commands;

        public CommandCollection()
        {
            commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        }

        public void Clear()
        {
            commands.Clear();
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

            if (commands.ContainsKey(name))
            {
                return commands[name];
            }
            else
            {
                var result = new Command(name);

                commands.Add(name, result);

                return result;
            }
        }

        public void Update(IState inputState)
        {
#if ASSERT
            if (inputState == null)
            {
                throw new ArgumentNullException("inputState");
            }
#endif

            foreach (var command in this)
            {
                command.Down = command.PrimaryBinding != null && command.PrimaryBinding.IsDown(inputState);
                command.Pressed = command.PrimaryBinding != null && command.PrimaryBinding.IsPressed(inputState);

                if (command.SecondaryBinding != null)
                {
                    command.Down = command.Down || command.SecondaryBinding.IsDown(inputState);
                    command.Pressed = command.Pressed || command.SecondaryBinding.IsPressed(inputState);
                }
            }
        }

        public IEnumerator<Command> GetEnumerator()
        {
            return commands.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return commands.Values.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;

namespace Project1
{
    public class Command
    {
        public ConsoleKey Key { get; private set; }
        public Func<string> ExecuteAction { get; private set; }
        public Func<bool> CanExecute { get; private set; }

        public Command(ConsoleKey key, Func<string> executeAction, Func<bool> canExecute = null)
        {
            Key = key;
            ExecuteAction = executeAction;
            CanExecute = canExecute ?? (() => true);
        }
    }

    public class CommandMap
    {
        private readonly Dictionary<ConsoleKey, Command> _commands = new Dictionary<ConsoleKey, Command>();

        public void Register(ConsoleKey key, Func<string> action, Func<bool> canExecute = null)
        {
            _commands[key] = new Command(key, action, canExecute);
        }

        public void Register(ConsoleKey key, Action action, Func<bool> canExecute = null)
        {
            Register(key, () => { action.Invoke(); return string.Empty; }, canExecute);
        }

        public string HandleInput(ConsoleKeyInfo keyInfo)
        {
            if (_commands.TryGetValue(keyInfo.Key, out var command))
            {
                if (command.CanExecute()) return command.ExecuteAction.Invoke();

                return $"The action for key [{keyInfo.Key}] is currently unavailable.";
            }

            return $"Unknown key: {keyInfo.Key}.";
        }
    }
}
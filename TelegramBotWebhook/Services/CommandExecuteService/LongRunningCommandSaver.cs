using System.Collections.Concurrent;
using TelegramBotWebhook.Command;

namespace TelegramBotWebhook.Services
{
    public class LongRunningCommandSaver
    {
        private readonly ConcurrentDictionary<long, ILongRunning> _usersCommands = new ConcurrentDictionary<long, ILongRunning>();

        public ILongRunning GetRunningCommand(long userId)
        {
            ILongRunning? runningCommand;
            if (_usersCommands.TryGetValue(userId, out runningCommand))
            {
                return runningCommand;
            }
            else
            {
                throw new ArgumentException("The running command for this user not found.");
            }
        }
        public void AddRunnningCommand(long userId, ILongRunning command)
        {
            _usersCommands.AddOrUpdate(userId, command, (userId, oldCommand) => command);
        }
        public bool ContainsCommand(long userId)
        {
            ILongRunning? command;
            return _usersCommands.TryGetValue(userId, out command);
        }
        public void RemoveCommand(long userId)
        {
            _usersCommands.Remove(userId, out ILongRunning? command);
        }
    }
}

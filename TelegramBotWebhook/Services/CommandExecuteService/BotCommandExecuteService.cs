using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        IServiceProvider _services;
        BotCommandLibrary _library;
        private BotCommand? _runningCommand;

        public BotCommandExecuteService(IServiceProvider services)
        {
            _services = services;
            _library = new BotCommandLibrary();
        }

        public Task<ExecuteResult> ExecuteCommand(string command)
        {
            ExecuteResult result;
            if (_library.CommandExists(command))
            {
                BotCommand botCommand = _library.GetCommandInstance(command);

                if (botCommand is ILongRunning)
                    _runningCommand = botCommand;
                if (botCommand is IServiceRequired serviceRequiredCommand)
                {
                    foreach (var serviceType in serviceRequiredCommand.RequiredServicesTypes)
                    {
                        serviceRequiredCommand.AddService(_services.GetRequiredService(serviceType));
                    }
                }
                
                result = botCommand.Execute(String.Empty).Result;
            }
            else
            {
                result = new ExecuteResult(ResultType.Text, "Command not found. Use /help to see the list of commands.");
            }
            return Task.FromResult(result);
        }
        public Task<ExecuteResult> HandleResponse(string response)
        {
            if (ExecuteIsOver())
                throw new Exception("No running command to receive and handle a response.");

            var longRunningCommand = _runningCommand as ILongRunning;

            ExecuteResult result;
            if (longRunningCommand is not null)
            {
                longRunningCommand.ExecuteIsOver += SetNullToRunningCommand;

                result = _runningCommand!.Execute(response).Result;

                longRunningCommand.ExecuteIsOver -= SetNullToRunningCommand;
            }
            else
            {
                result = _runningCommand!.Execute(response).Result;
            }
            
            return Task.FromResult(result);
        }
        public bool ExecuteIsOver() => _runningCommand is null;
        private void SetNullToRunningCommand() => _runningCommand = null;
    }
}

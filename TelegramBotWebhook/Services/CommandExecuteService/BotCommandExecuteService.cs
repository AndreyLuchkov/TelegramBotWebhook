using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        IServiceProvider _services;
        BotCommandLibrary _library;
        private ILongRunning? _runningCommand;

        public BotCommandExecuteService(IServiceProvider services)
        {
            _services = services;
            _library = new BotCommandLibrary();
        }

        public async Task<ExecuteResult> ExecuteCommand(string command, long userId)
        {
            ExecuteResult result;
            if (_library.CommandExists(command))
            {
                BotCommand botCommand = _library.GetCommandInstance(command);

                if (botCommand is ILongRunning longRunningCommand)
                {
                    _runningCommand = longRunningCommand;
                }
                if (botCommand is IServiceRequired serviceRequiredCommand)
                {
                    AddRequiredServices(serviceRequiredCommand);
                }
                if (botCommand is ISessionDepended sessionDependedCommand)
                {
                    sessionDependedCommand.Session = Session.GetInstance(userId);
                }
                
                result = await botCommand.Execute(String.Empty);
            }
            else
            {
                result = new ExecuteResult(ResultType.Text, "Команда не найдена. Воспользуйтесь /help , чтобы получить список доступных команд.");
            }
            
            if (_runningCommand is not null)
                result.RemoveKeyboard = false;

            return result;
        }
        private void AddRequiredServices(IServiceRequired serviceRequiredCommand)
        {
            foreach (var serviceType in serviceRequiredCommand.RequiredServicesTypes)
            {
                serviceRequiredCommand.AddService(_services.GetRequiredService(serviceType));
            }
        }
        public async Task<ExecuteResult> HandleResponse(string response, long userId)
        {
            if (ExecuteIsOver())
                throw new Exception("No running command to receive and handle the response.");

            _runningCommand!.ExecuteIsOver += ClearRunningCommand;

            if (_runningCommand is ISessionDepended sessionDependedCommand)
            {
                sessionDependedCommand.Session = Session.GetInstance(userId);
            }

            return await _runningCommand!.Execute(response);
        }
        private void ClearRunningCommand() => _runningCommand = null;
        public bool ExecuteIsOver() => _runningCommand is null;
    }
}

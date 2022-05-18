using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        private readonly IServiceProvider _services;
        private ILongRunning? _runningCommand;

        public BotCommandExecuteService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<ExecuteResult> ExecuteCommand(string command, long userId)
        {
            ExecuteResult result;
            if (BotCommandLibrary.CommandExists(command))
            {
                BotCommand botCommand = BotCommandLibrary.GetCommandInstance(command);

                if (botCommand is ILongRunning longRunningCommand)
                {
                    _runningCommand = longRunningCommand;
                }
                if (botCommand is IServiceRequired serviceRequiredCommand)
                {
                    AddRequiredServices(serviceRequiredCommand);
                }
                
                result = await botCommand.Execute(userId.ToString());
            }
            else
            {
                result = new ExecuteResult(ResultType.Text, "Команда не найдена. Воспользуйтесь /help, чтобы получить список доступных команд.");
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
            if (IsExecuteOver())
                throw new Exception("No running command to receive and handle the response.");

            _runningCommand!.ExecuteIsOver += ClearRunningCommand;

            return await _runningCommand!.Execute(response);
        }
        private void ClearRunningCommand() => _runningCommand = null;
        public bool IsExecuteOver() => _runningCommand is null;
    }
}

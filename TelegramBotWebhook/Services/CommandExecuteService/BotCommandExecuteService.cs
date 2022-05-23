using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        private readonly BotCommandLibrary _commandLibrary;
        private readonly IServiceProvider _services;
        private readonly LongRunningCommandSaver _commandSaver;

        public BotCommandExecuteService(IServiceProvider services, LongRunningCommandSaver commandSaver)
        {
            _commandLibrary = new BotCommandLibrary();
            _services = services;
            _commandSaver = commandSaver;
        }

        public async Task<ExecuteResult> ExecuteCommand(string command, long userId)
        {
            ExecuteResult result;
            if (_commandLibrary.CommandExists(command))
            {
                BotCommand botCommand = _commandLibrary.GetCommandInstance(command);

                if (botCommand is ILongRunning longRunningCommand)
                {
                    _commandSaver.AddRunnningCommand(userId, longRunningCommand);
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
            
            if (_commandSaver.ContainsCommand(userId))
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
            if (IsExecuteOver(userId))
                throw new Exception("No running command for this user to receive and handle the response.");

            var runningCommand = _commandSaver.GetRunningCommand(userId);

            runningCommand.ExecuteIsOver += () => _commandSaver.RemoveCommand(userId);

            return await runningCommand.Execute(response);
        }
        public bool IsExecuteOver(long userId) => !_commandSaver.ContainsCommand(userId);
    }
}

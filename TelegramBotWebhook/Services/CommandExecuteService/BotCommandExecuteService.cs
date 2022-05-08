using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        IServiceProvider services;
        private BotCommand? runningCommand;

        public BotCommandExecuteService(IServiceProvider services)
        {
            this.services = services;
        }

        public Task<ExecuteResult> ExecuteCommand(string command)
        {
            ExecuteResult result;
            if (BotCommandLibrary.CommandExists(command))
            {
                BotCommand botCommand = BotCommandLibrary.GetCommandInstance(command);

                if (botCommand is ILongRunning)
                    runningCommand = botCommand;
                if (botCommand is IServiceRequired serviceRequiredCommand)
                {
                    foreach (var serviceType in serviceRequiredCommand.RequiredServicesTypes)
                    {
                        serviceRequiredCommand.Services.Add(services.GetRequiredService(serviceType));
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

            if (runningCommand is ILongRunning longRunningCommand)
            {
                longRunningCommand.ExecuteIsOver += () => runningCommand = null;
            }

            ExecuteResult result = runningCommand!.Execute(response).Result;
            return Task.FromResult(result);
        }
        public bool ExecuteIsOver() => runningCommand is null;
    }
}

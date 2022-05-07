using TelegramBotWebhook.Command;
using TelegramBotWebhook.Command.BotCommand;

namespace TelegramBotWebhook.Services
{
    public class BotCommandExecuteService : ICommandExecuteService<ExecuteResult>
    {
        private BotCommand? runningCommand;

        public Task<ExecuteResult> ExecuteCommand(string command)
        {
            ExecuteResult result;
            if (BotCommandLibrary.CommandExists(command))
            {
                BotCommand botCommand = BotCommandLibrary.GetCommandInstance(command);

                if (botCommand is ILongRunningCommand)
                    runningCommand = botCommand;
                
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

            if (runningCommand is ILongRunningCommand longRunningCommand)
            {
                longRunningCommand.ExecuteIsOver += () => runningCommand = null;
            }

            ExecuteResult result = runningCommand!.Execute(response).Result;
            return Task.FromResult(result);
        }
        public bool ExecuteIsOver() => runningCommand is null;
    }
}

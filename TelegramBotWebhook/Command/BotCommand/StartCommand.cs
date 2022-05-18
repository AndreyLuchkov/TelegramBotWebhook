using System.Text;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class StartCommand : BotCommand
    {
        internal StartCommand() : base("/start") { }

        public override Task<ExecuteResult> Execute(string _)
        {
            StringBuilder resultMessage = new StringBuilder("Используйте эти команды, чтобы контролировать бота: \n");

            var commandsNames = BotCommandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start");

            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            resultMessage.Append("\nС помощью /help можно получить список доступных команд.");

            return Task.FromResult(new ExecuteResult(ResultType.Text, resultMessage));
        }
    }
}

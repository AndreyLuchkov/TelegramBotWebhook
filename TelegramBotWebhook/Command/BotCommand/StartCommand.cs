using System.Text;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class StartCommand : BotCommand
    {
        public StartCommand() : base("start") { }

        public override object Clone() => new StartCommand();

        public override Task<ExecuteResult> Execute(string _)
        {
            StringBuilder resultMessage = new StringBuilder("You can control me by sending these commands: \n");

            string[] commandsNames = BotCommandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start").ToArray();

            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            resultMessage.Append("\nUse /help to get the list of commands.");

            return Task.FromResult(new ExecuteResult(ResultType.Text, resultMessage));
        }
    }
}

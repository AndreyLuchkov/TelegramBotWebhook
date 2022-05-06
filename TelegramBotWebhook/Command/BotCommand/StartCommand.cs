namespace TelegramBotWebhook.Command.BotCommand
{
    public class StartCommand : BotCommand
    {
        public StartCommand() : base(
            text: "start",
            isLongRunning: false) { }

        public override object Clone() => new StartCommand();

        public override Task<ExecuteResult> Execute(string _)
        {
            var result = new ExecuteResult(ResultType.Text, "You can control me by sending this commands: \n");

            string[] commandsNames = BotCommandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start").ToArray();

            foreach (var commandName in commandsNames)
            {
                result.Message += commandName + '\n';
            }
            result.Message += "\nUse /help to get the list of commands.";

            return Task.FromResult(result);
        }
    }
}

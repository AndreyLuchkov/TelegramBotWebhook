namespace TelegramBotWebhook.Command.BotCommand
{
    internal class HelpCommand : BotCommand
    {
        public HelpCommand() : base(
            text: "help",
            isLongRunning: false) { }
        
        public override object Clone() => new HelpCommand();
        
        public override Task<ExecuteResult> Execute(string _)
        {
            var result = new ExecuteResult(ResultType.Text, "List of commands: \n");

            string[] commandsNames = BotCommandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start").ToArray();

            foreach (var commandName in commandsNames)
            {
                result.Message += commandName + '\n';
            }

            return Task.FromResult(result);
        }
    }
}

using System.Text;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class HelpCommand : BotCommand
    {
        public HelpCommand() : base("help") { }

        public override object Clone() => new HelpCommand();
        
        public override Task<ExecuteResult> Execute(string _)
        {
            var resultMessage = new StringBuilder("The list of commands: \n");

            string[] commandsNames = BotCommandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start").ToArray();
            
            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            
            return Task.FromResult(new ExecuteResult(ResultType.Text, resultMessage));
        }
    }
}

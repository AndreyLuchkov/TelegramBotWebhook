using System.Text;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class HelpCommand : BotCommand
    {
        public HelpCommand() : base("help") { }
        
        public override Task<ExecuteResult> Execute(string _)
        {
            var resultMessage = new StringBuilder("The list of commands: \n");

            BotCommandLibrary library = new BotCommandLibrary();
            string[] commandsNames = library.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start").ToArray();
            
            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            
            return Task.FromResult(new ExecuteResult(ResultType.Text, resultMessage));
        }
    }
}

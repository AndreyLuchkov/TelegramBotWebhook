using System.Text;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class HelpCommand : BotCommand
    {
        internal HelpCommand() : base("help") { }
        
        public override Task<ExecuteResult> Execute(string _)
        {
            var resultMessage = new StringBuilder("Список команд: \n");

            BotCommandLibrary commandLibrary = new();
            var commandsNames = commandLibrary.GetAllCommandNames()
                .Where((name) => name != "/help" && name != "/start");
            
            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            
            return Task.FromResult(new ExecuteResult(ResultType.Text, resultMessage));
        }
    }
}

namespace TelegramBotWebhook.Command.BotCommand
{
    static public class BotCommandLibrary
    {
        private static Dictionary<string, BotCommand> library = new Dictionary<string, BotCommand>
        {
            ["/help"] = new HelpCommand(),
            ["/close"] = new CloseCommand(),
            ["/start"] = new StartCommand(),
        };

        static public bool CommandExists(string command) => library.ContainsKey(command);
        static public BotCommand GetCommandInstance(string command)
        {
            try
            {
                return (BotCommand)library[command].Clone();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"The command {command} not found in the library.");
            }
        }
        static public string[] GetAllCommandNames() => library.Keys.ToArray();
    }
}

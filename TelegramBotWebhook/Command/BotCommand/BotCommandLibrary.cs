namespace TelegramBotWebhook.Command.BotCommand
{
    static public class BotCommandLibrary
    {
        static private readonly Dictionary<string, BotCommand> library = new Dictionary<string, BotCommand>
        {
            ["/help"] = new HelpCommand(),
            ["/close"] = new CloseCommand(),
            ["/start"] = new StartCommand(),
            ["/unread"] = new UnreadCommand(),
            ["/lessons"] = new LessonsCommand(),
            ["/login"] = new LoginCommand(),
        };

        public static bool CommandExists(string command) => library.ContainsKey(command);
        public static BotCommand GetCommandInstance(string command)
        {
            try
            {
                return library[command];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"The command {command} not found in the library.");
            }
        }
        public static string[] GetAllCommandNames() => library.Keys.ToArray();
    }
}

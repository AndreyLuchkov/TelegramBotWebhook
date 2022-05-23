namespace TelegramBotWebhook.Command.BotCommand
{
    public class BotCommandLibrary
    {
        private readonly Dictionary<string, BotCommand> library = new Dictionary<string, BotCommand>
        {
            ["/help"] = new HelpCommand(),
            ["/start"] = new StartCommand(),
            ["/unread"] = new UnreadCommand(),
            ["/lessons"] = new LessonsCommand(),
            ["/login"] = new LoginCommand(),
            ["/settings"] = new SettingsCommand(),
            [";mpeiaccount"] = new MPEIAccountCommand(),
            [";mpeiaccountsave"] = new MPEIAccountSaveCommand(),
            [";mpeiaccountunlog"] = new MPEIAccountUnlogCommand(),
        };

        public bool CommandExists(string command) => library.ContainsKey(command);
        public BotCommand GetCommandInstance(string command)
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
        public string[] GetAllCommandNames() => library.Keys.Where((commandName) => commandName.First() == '/').ToArray();
    }
}

namespace TelegramBotWebhook.Command.BotCommand
{
    public abstract class BotCommand : ICommand
    {
        public string? Text { get; }
        public string Prefix { get; } = "/";

        protected BotCommand(string? text)
        {
            Text = text;
        }
        protected BotCommand(string prefix, string? text) : this(text)
        {
            Prefix = prefix;
        }

        public abstract Task<ExecuteResult> Execute(string option);
    }
}

namespace TelegramBotWebhook.Command.BotCommand
{
    public abstract class BotCommand : ICommand
    {
        public string? Text { get; }
        public string Prefix { get; } = "/";

        protected BotCommand(string? text)
        {
            Text = text?.TrimStart('/');
        }

        public abstract Task<ExecuteResult> Execute(string option);
    }
}

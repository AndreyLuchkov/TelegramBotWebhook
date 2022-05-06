namespace TelegramBotWebhook.Command.BotCommand
{
    public abstract class BotCommand : ICommand<string, ExecuteResult>, ICloneable
    {
        public string? Text { get; }
        public string Prefix { get; } = "/";
        public bool IsLongRunning { get; }

        protected BotCommand(string? text, bool isLongRunning)
        {
            Text = text?.TrimStart('/');
            IsLongRunning = isLongRunning;
        }

        public abstract Task<ExecuteResult> Execute(string option);
        public abstract object Clone();
    }
}

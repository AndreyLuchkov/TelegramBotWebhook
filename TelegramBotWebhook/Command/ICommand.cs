namespace TelegramBotWebhook.Command
{
    public interface ICommand 
    {
        string? Text { get; }
        string Prefix { get; }

        public Task<ExecuteResult> Execute(string option);
    }
}

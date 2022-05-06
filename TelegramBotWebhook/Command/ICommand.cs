namespace TelegramBotWebhook.Command
{
    public interface ICommand<TOption, TResult> where TResult: class
    {
        string? Text { get; }
        string Prefix { get; }

        public Task<TResult> Execute(TOption option);
    }
}

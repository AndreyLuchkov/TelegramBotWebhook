namespace TelegramBotWebhook.Services
{
    public interface IMessageSendingService<TOptions> where TOptions: class
    {
        public Task SendMessage(IChat chat, TOptions options);
    }
}

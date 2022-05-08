namespace TelegramBotWebhook.Services
{
    public interface IEmailReadService
    {
        public Task<DirectoryInfo> ReadLetters();
    }
}

namespace TelegramBotWebhook.Services
{
    public interface ISessionService<T> where T: class
    {
        public Task StartSession(long userId);
        public Task<T> GetSession(long userId);
    }
}

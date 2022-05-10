using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public interface IEmailReadService
    {
        public Task<IEnumerable<Letter>> ReadLetters();
    }
}

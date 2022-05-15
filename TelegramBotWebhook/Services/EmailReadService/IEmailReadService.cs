using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public interface IEmailReadService
    {
        public Task<IEnumerable<LetterRecord>> GetLetters(Session session);
    }
}

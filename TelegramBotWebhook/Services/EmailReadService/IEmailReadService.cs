using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public interface IEmailReadService
    {
        public Task<IEnumerable<LetterRecord>> GetLetters(MPEISession session);
    }
}

using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public interface IEmailLetterReadService<TResult> where TResult: class
    {
        public Task<TResult> ReadLetter(Session session, LetterRecord letterRecord);
        public Task<IEnumerable<TResult>> ReadLetters(Session session, IEnumerable<LetterRecord> letterRecords);
    }
}

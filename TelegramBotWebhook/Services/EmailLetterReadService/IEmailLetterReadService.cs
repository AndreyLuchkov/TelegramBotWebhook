using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public interface IEmailLetterReadService<TResult> where TResult: class
    {
        public Task<TResult> ReadLetter(MPEISession session, LetterRecord letterRecord);
        public Task<IEnumerable<TResult>> ReadLetters(MPEISession session, IEnumerable<LetterRecord> letterRecords);
    }
}
